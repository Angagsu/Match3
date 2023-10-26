using System;
using System.Collections.Generic;
using UnityEngine;

public class CellFactory : MonoBehaviour
{
    private BoardService boardService;

    [Header("BoardRects")]
    [SerializeField] private RectTransform boardRect;
    [SerializeField] private RectTransform killedBoardRect;

    [Header("Prefabs")]
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private KilledCell killedCellPrefab;


    private readonly List<KilledCell> killedCells = new List<KilledCell>();

    public void InstantiateBoard(BoardService boardService, CellMover cellMover)
    {
        this.boardService = boardService;

        for (int y = 0; y < Config.BoardHeight; y++)
        {
            for (int x = 0; x < Config.BoardWidth; x++)
            {
                var point = new Point(x, y);
                var cellData = boardService.GetCellAtPoint(point);
                var cellType = cellData.cellType;
                if (cellType <= 0)
                {
                    continue;
                }

                var cell = InstantiateCell();
                cell.Rect.anchoredPosition = BoardService.GetBoardPositionFromPoint(point);
                cell.Initialize(
                    new CellData(cellType, new Point(x, y)), 
                    boardService.CellSprites[(int)cellType - 1],
                    cellMover
                    );
                cellData.SetCell(cell);
            }
        }
    }

    

    public Cell InstantiateCell()
    => Instantiate(cellPrefab, boardRect);

    public void KillCell(Point point)
    {
        var availableCells = new List<KilledCell>();
        foreach (var killedCell in killedCells)
        {
            if (!killedCell.isFalling)
            {
                availableCells.Add(killedCell);
            }
        }

        KilledCell showedKilledCell;

        if (availableCells.Count > 0)
        {
            showedKilledCell = availableCells[0];
        }
        else
        {
            var killedCell = Instantiate(killedCellPrefab, killedBoardRect);
            showedKilledCell = killedCell;
            killedCells.Add(killedCell);
        }

        var cellTypeIndex = (int)boardService.GetCellTypeAtPoint(point) - 1;

        if (showedKilledCell != null && cellTypeIndex >= 0 && cellTypeIndex < boardService.CellSprites.Length)
        {
            showedKilledCell.Initialize(
                boardService.CellSprites[cellTypeIndex], 
                BoardService.GetBoardPositionFromPoint(point)
                );
        }
    }
}


