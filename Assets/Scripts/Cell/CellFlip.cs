using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFlip 
{
    private readonly Cell firstCell;
    private readonly Cell secondCell;

    public CellFlip(Cell firstCell, Cell secondCell)
    {
        this.firstCell = firstCell;
        this.secondCell = secondCell;
    }

    public Cell GetOtherCell(Cell cell)
    {
        if (cell == firstCell)
        {
            return secondCell;
        }

        if (cell == secondCell)
        {
            return firstCell;
        }

        return null;
    }
}
