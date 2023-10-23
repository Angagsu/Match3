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
    public Point point { get; private set; }
    public Cell cell { get; private set; }

    public CellData(CellType cellType, Point point)
    {
        this.cellType = cellType;
        this.point = point;
    }
}
