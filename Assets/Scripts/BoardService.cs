using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CellFactory))]
public class BoardService : MonoBehaviour
{
    public ArrayLayout BoardLayout;
    public Sprite[] CellSprites => cellSprites;
    
    [SerializeField] private Sprite[] cellSprites;

    private CellData[,] board;
    private CellFactory cellFactory;
    private MatchMachine matchMachine;
    private CellMover cellMover;

    private readonly List<Cell> updatingCells = new List<Cell>();

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
            updatingCells.Remove(cell);
        }
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
            :  availableCellTypes[Random.Range(0, availableCellTypes.Count)];
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

    private CellData.CellType GetRandomCellType()
        => (CellData.CellType)(Random.Range(0, cellSprites.Length) + 1);
    
}
