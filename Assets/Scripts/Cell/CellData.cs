using System;

public class CellData 
{
    public enum CellType
    {
        Hole = -1,
        Blank = 0,
        Apple = 1,
        Lemon = 2,
        Bread = 3,
        Broccoli = 4,
        Coconut = 5
    }

    public CellType cellType { get; set; }
    public Point point { get; set; }
    public Cell cell { get; private set; }

    public CellData(CellType cellType, Point point)
    {
        this.cellType = cellType;
        this.point = point;
    }

    internal void SetCell(Cell otherCell)
    {
        cell = otherCell;

        if (cell == null)
        {
            cellType = CellType.Blank;
        }
        else
        {
            cellType = otherCell.CellType;
            cell.SetCellPoint(point);
        }
    }
}
