using UnityEngine;

public class CellFactory : MonoBehaviour
{
    private BoardService boardService;

    [SerializeField] private RectTransform boardRect;
    [SerializeField] private Cell cellPrefab;

    public void InstantiateBoard(BoardService boardService, CellMover cellMover)
    {
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
                cell.Initialize(cellData, boardService.CellSprites[(int)cellType - 1], cellMover);
            }
        }
    }

    

    private Cell InstantiateCell()
    => Instantiate(cellPrefab, boardRect);
}
