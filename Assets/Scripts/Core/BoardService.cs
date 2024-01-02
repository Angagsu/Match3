using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
    #region Events

    public event Action Matched;
    public event Action NotMatched;

    #endregion

    public ArrayLayout BoardLayout;
    public Sprite[] CellSprites => cellSprites;
    
    [SerializeField] private Sprite[] cellSprites;
    [SerializeField] private ParticleSystem matchFXPrefab;
    [SerializeField] private ScoreTextUI scoreService;

    private CellData[,] board;
    private CellFactory cellFactory;
    private MatchMachine matchMachine;
    private CellMover cellMover;

    private readonly int[] fillingCellsCountByColumn = new int[Config.BoardWidth];
    private readonly List<Cell> updatingCells = new List<Cell>();
    private readonly List<Cell> deadCells = new List<Cell>();
    private readonly List<CellFlip> flippedCells = new List<CellFlip>();
    private readonly List<ParticleSystem> matchFXs = new List<ParticleSystem>();


    private void Awake()
    {
        cellFactory = GetComponent<CellFactory>();
        matchMachine = new MatchMachine(this);
        cellMover = new CellMover(this);
    }
    private void Start()
    {
        InitializeBoard();
        VerifyBoardOnMatches();
        cellFactory.InstantiateBoard(this, cellMover);
    }

    private void Update()
    {
        cellMover.Update();

        var finishedUpdating = new List<Cell>();

        foreach (var cell in updatingCells)
        {
            if (!cell.UpdateCell())
            {
                finishedUpdating.Add(cell);
            }            
        }

        foreach (var cell in finishedUpdating)
        {
            var x = cell.Point.x;
            fillingCellsCountByColumn[x] = 
                Mathf.Clamp(fillingCellsCountByColumn[x] - 1, 0, Config.BoardWidth);

            var flip = GetFlip(cell);
            var connectedPoints = matchMachine.GetMatchedPoints(cell.Point, true);
            Cell flippedCell = null;

            if (flip != null)
            {
                flippedCell = flip.GetOtherCell(cell);
                MatchMachine.AddPoints(
                    ref connectedPoints,
                    matchMachine.GetMatchedPoints(flippedCell.Point, true)
                    );
            }

            if (connectedPoints.Count == 0)
            {
                if (flippedCell != null)
                {
                    NotMatched?.Invoke();

                    FlipCells(cell.Point, flippedCell.Point, false);
                }  
            }
            else
            {
                Matched?.Invoke();

                ParticleSystem matchFx;

                if (matchFXs.Count > 0 && matchFXs[0].isStopped)
                {
                    matchFx = matchFXs[0];
                    matchFXs.RemoveAt(0);
                }
                else
                {
                    matchFx = Instantiate(matchFXPrefab, transform);
                }

                matchFXs.Add(matchFx);
                matchFx.Play();
                matchFx.transform.position = cell.Rect.transform.position;

                foreach (var connectedPoint in connectedPoints)
                {
                    cellFactory.KillCell(connectedPoint);
                    var cellAtPoint = GetCellAtPoint(connectedPoint);
                    var connectedCell = cellAtPoint.cell;

                    if (connectedCell != null)
                    {
                        connectedCell.gameObject.SetActive(false);
                        deadCells.Add(connectedCell);
                    }

                    cellAtPoint.SetCell(null);
                }

                scoreService.AddScore(connectedPoints.Count);

                ApplyGravityToBoard();
            }

            flippedCells.Remove(flip);
            updatingCells.Remove(cell);
        }
    }

    private void ApplyGravityToBoard()
    {
        for (int x = 0; x < Config.BoardWidth; x++)
        {
            for (int y = Config.BoardHeight - 1; y >= 0; y--)
            {
                var point = new Point(x, y);
                var cellData = GetCellAtPoint(point);
                var cellTypeAtPoint = GetCellTypeAtPoint(point);

                if (cellTypeAtPoint != 0)
                {
                    continue;
                }

                for (int newY = y - 1; newY >= -1; newY--)
                {
                    var nextPoint = new Point(x, newY);
                    var nextCellType = GetCellTypeAtPoint(nextPoint);

                    if (nextCellType == 0)
                    {
                        continue;
                    }

                    if (nextCellType != CellData.CellType.Hole)
                    {
                        var cellAtPoint = GetCellAtPoint(nextPoint);
                        var cell = cellAtPoint.cell;

                        cellData.SetCell(cell);
                        updatingCells.Add(cell);

                        cellAtPoint.SetCell(null);
                    }
                    else
                    {
                        var cellType = GetRandomCellType();
                        var fallPoint = new Point(x, -1 - fillingCellsCountByColumn[x]);
                        Cell cell;
                        if (deadCells.Count > 0)
                        {
                            var revivedCell = deadCells[0];
                            revivedCell.gameObject.SetActive(true);
                            cell = revivedCell;
                            deadCells.RemoveAt(0);
                        }
                        else
                        {
                            cell = cellFactory.InstantiateCell();
                        }

                        cell.Initialize(new CellData(cellType, point), cellSprites[(int)cellType - 1], cellMover);
                        cell.Rect.anchoredPosition = GetBoardPositionFromPoint(fallPoint);

                        var holeCell = GetCellAtPoint(point);
                        holeCell.SetCell(cell);
                        ResetCell(cell);
                        fillingCellsCountByColumn[x]++;
                    }

                    break;
                }
            }
        }
    }

    public void FlipCells(Point firstPoint, Point secondPoint, bool main)
    {
        if (GetCellTypeAtPoint(firstPoint) < 0)
        {
            return;
        }

        var firstCellData = GetCellAtPoint(firstPoint);
        var firstCell = firstCellData.cell;

        if (GetCellTypeAtPoint(secondPoint) > 0)
        {
            var secondCellData = GetCellAtPoint(secondPoint);
            var secondCell = secondCellData.cell;

            firstCellData.SetCell(secondCell);
            secondCellData.SetCell(firstCell);

            if (main)
            {
                flippedCells.Add(new CellFlip(firstCell, secondCell));
            }

            updatingCells.Add(firstCell);
            updatingCells.Add(secondCell);
        }
        else
        {
            ResetCell(firstCell);
        }
    }

    private CellFlip GetFlip(Cell cell)
    {
        foreach (var flip in flippedCells)
        {
            if (flip.GetOtherCell(cell) != null)
            {
                return flip;
            }
        }
        return null;
    }

    public void ResetCell(Cell cell)
    {
        cell.ResetPosition();
        updatingCells.Add(cell);
    }

    private void InitializeBoard()
    {
        board = new CellData[Config.BoardWidth, Config.BoardHeight];

        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                board[x, y] = new CellData(
                    BoardLayout.rows[y].row[x] ? CellData.CellType.Hole : GetRandomCellType(),
                    new Point(x, y)
                    );
            }
        }
    }

    private void VerifyBoardOnMatches()
    {
        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                var point = new Point(x, y);
                var cellTypeAtPoint = GetCellTypeAtPoint(point);
                if (cellTypeAtPoint <= 0)
                {
                    continue;
                }

                var removeCellTypes = new List<CellData.CellType>();

                while (matchMachine.GetMatchedPoints(point, true).Count > 0)
                {
                    if (removeCellTypes.Contains(cellTypeAtPoint) == false)
                    {
                        removeCellTypes.Add(cellTypeAtPoint);
                    }

                    SetCellTypeAtPoint(point, GetNewCellType(ref removeCellTypes));
                }
            }
        }
    }

    private void SetCellTypeAtPoint(Point point, CellData.CellType newCellType)
    {
        board[point.x, point.y].cellType = newCellType;
    }

    private CellData.CellType GetNewCellType(ref List<CellData.CellType> removeCellTypes)
    {
        var availableCellTypes = new List<CellData.CellType>();
        for (int i = 0; i < CellSprites.Length; i++)
        {
            availableCellTypes.Add((CellData.CellType)i + 1);
        }
        foreach (var removeCellType in removeCellTypes)
        {
            availableCellTypes.Remove(removeCellType);
        }

        return availableCellTypes.Count <= 0 
            ? CellData.CellType.Blank 
            :  availableCellTypes[UnityEngine.Random.Range(0, availableCellTypes.Count)];
    }

    public CellData.CellType GetCellTypeAtPoint(Point point)
    {
        if (point.x < 0 || point.x >= Config.BoardWidth || point.y < 0 || point.y >= Config.BoardHeight)
        {
            return CellData.CellType.Hole;
        }

        return board[point.x, point.y].cellType;
    }

    public CellData GetCellAtPoint(Point point)
    {
        return board[point.x, point.y];
    }

    public static Vector2 GetBoardPositionFromPoint(Point point)
    {
        return new Vector2(
            Config.PieceSize / 2 + Config.PieceSize * point.x,
            -Config.PieceSize / 2 - Config.PieceSize * point.y
            );
    }

    public void DisableBoardService()
    {
        cellMover.IsGamePaused = true;
        enabled = false;
    }

    public void EnableBoardService()
    {
        cellMover.IsGamePaused = false;
        enabled = true;
    }

    private CellData.CellType GetRandomCellType()
        => (CellData.CellType)(UnityEngine.Random.Range(0, cellSprites.Length) + 1);
    
}
