using System;
using System.Collections.Generic;

public class MatchMachine 
{
    private readonly BoardService boardService;
    private readonly Point[] directions =
    {
        Point.Up, Point.Right, Point.Down, Point.Left
    };

    public MatchMachine(BoardService boardService)
    {
        this.boardService = boardService;
    }

    public List<Point> GetMatchedPoints(Point point, bool main)
    {
        var connectedPoints = new List<Point>();
        var cellTypeAtPoint = boardService.GetCellTypeAtPoint(point);

        CheckForDirectionMatch(ref connectedPoints, point, cellTypeAtPoint);
        CheckForMiddleOfMatch(ref connectedPoints, point, cellTypeAtPoint);
        CheckForSquareMatch(ref connectedPoints, point, cellTypeAtPoint);

        if (main)
        {
            for (int i = 0; i < connectedPoints.Count; i++)
            {
                AddPoints(ref connectedPoints, GetMatchedPoints(connectedPoints[i], false));
            }
        }

        return connectedPoints;
    }

    private void CheckForSquareMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        for (int i = 0; i < 4; i++)
        {
            var square = new List<Point>();

            var nextCellIndex = i + 1;
            nextCellIndex = nextCellIndex > 3 ? 0 : nextCellIndex;

            Point[] checkPoints =
            {
                Point.AddPoint(point, directions[i]),
                Point.AddPoint(point, directions[nextCellIndex]),
                Point.AddPoint(point, Point.AddPoint(directions[i], directions[nextCellIndex]))
            };

            foreach (var checkpoint in checkPoints)
            {
                if (boardService.GetCellTypeAtPoint(checkpoint) == cellTypeAtPoint)
                {
                    square.Add(checkpoint);
                }
            }

            if (square.Count > 2)
            {
                AddPoints(ref connectedPoints, square);
            }
        }
    }

    private void CheckForMiddleOfMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        for (int i = 0; i < 2; i++)
        {
            var line = new List<Point>();

            Point[] checkPoints =
            {
                Point.AddPoint(point, directions[i]),
                Point.AddPoint(point, directions[i + 2])
            };

            foreach (var checkPoint in checkPoints)
            {
                if (boardService.GetCellTypeAtPoint(checkPoint) == cellTypeAtPoint)
                {
                    line.Add(checkPoint);
                }
            }

            if (line.Count > 1)
            {
                AddPoints(ref connectedPoints, line);
            }
        }
    }

    private void CheckForDirectionMatch(ref List<Point> connectedPoints, Point point, CellData.CellType cellTypeAtPoint)
    {
        foreach (var direction in directions)
        {
            var line = new List<Point>();

            for (int i = 1; i < 3; i++)
            {
                var checkpoint = Point.AddPoint(point, Point.Multiply(direction, i));
                if (boardService.GetCellTypeAtPoint(checkpoint) == cellTypeAtPoint)
                {
                    line.Add(checkpoint);        
                }
            }

            if (line.Count > 1)
            {
                AddPoints(ref connectedPoints, line);
            }
        }
    }

    public static void AddPoints(ref List<Point> points, List<Point> addPoints)
    {       
        foreach (var addPoint in addPoints)
        {
            var doAdd = true;

            foreach (var point in points)
            {
                if (point.Equals(addPoint))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd)
            {
                points.Add(addPoint);
            }
        }
    }

    
}
