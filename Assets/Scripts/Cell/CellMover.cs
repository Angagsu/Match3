using System;
using UnityEngine;

public class CellMover
{
    private Cell movingCell;
    private Point newPoint;
    private Vector2 mouseStartPosition;
    private BoardService boardService;

    public CellMover(BoardService boardService)
    {
        this.boardService = boardService;
    }
    public void Update()
    {
        if (movingCell == null)
        {
            return;
        }

        var mousPosition = (Vector2)Input.mousePosition - mouseStartPosition;
        var mouseDirection = mousPosition.normalized;
        var absoluteDirection = new Vector2(MathF.Abs(mousPosition.x), Mathf.Abs(mousPosition.y));

        newPoint = Point.Clone(movingCell.Point);
        var addPoint = Point.Zero;

        if (mousPosition.magnitude > Config.PieceSize / 4)
        {
            if (absoluteDirection.x > absoluteDirection.y)
            {
                addPoint = new Point(mousPosition.x > 0 ? 1 : -1, 0);
            }
            else
            {
                addPoint = new Point(0, mousPosition.y > 0 ? 1 : -1);
            }
        }

        newPoint.Add(addPoint);

        var position = BoardService.GetBoardPositionFromPoint(movingCell.Point);

        if (!newPoint.Equals(movingCell.Point))
        {
            position += Point.Multiply(addPoint, Config.PieceSize / 2).ToVector();           
        }
        movingCell.MoveToPosition(position);
    }

    public void MoveCell(Cell cell)
    {
        if (movingCell != null)
        {
            return;
        }
        movingCell = cell;
        mouseStartPosition = Input.mousePosition;
    }

    public void DropCell()
    {
        if (movingCell == null)
        {
            return;
        }
        boardService.ResetCell(movingCell);
        movingCell = null;
    }
}