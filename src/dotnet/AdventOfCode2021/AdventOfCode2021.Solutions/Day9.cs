namespace AdventOfCode2021.Solutions;

public sealed partial class Day9 : IDay
{
    private record struct Point(int X, int Y)
    {
        public override string ToString()
        {
            return $"x={X},y={Y}";
        }
    }
    
    public int DayNumber => 9;
    
    public long CalculatePartOne()
    {
        var data = ParseInput();

        var points = from x in Enumerable.Range(0, data.GetLength(0))
            from y in Enumerable.Range(0, data.GetLength(1))
            where IsLower(x, y, data)
            select data[x, y] + 1L;

        return points.Aggregate(0L, (x1, x2) => x1 + x2);
    }

    public long CalculatePartTwo()
    {
        var data = ParseInput();
        var basinSizes = new List<long>();

        foreach (var x in Enumerable.Range(0, data.GetLength(0)))
        {
            foreach (var y in Enumerable.Range(0, data.GetLength(1)))
            {
                if (!IsLower(x, y, data))
                {
                    continue;
                }
                
                var point = new Point(x, y);
                basinSizes.Add(GetBasin(point, data));
            }
        }

        var maxSizes = basinSizes.OrderByDescending(s => s).Take(3).ToArray();
        return maxSizes.Aggregate((x1, x2) => x1 * x2);
    }

    private static long GetBasin(Point point, int[,] data)
    {
        var lengthX = data.GetLength(0);
        var lengthY = data.GetLength(1);

        var layer = new List<Point> { point };
        var basin = new HashSet<Point> { point };
        
        while (true)
        {
            var adjacent = layer.SelectMany(
                    p => GetAdjacent(p.X, p.Y)
                        .Where(p => p.X >= 0 && p.Y >= 0 && p.X < lengthX && p.Y < lengthY)
                        .Where(adj =>
                    {
                        var value = data[adj.X, adj.Y];
                        return value != 9 && (value > data[p.X, p.Y]);
                    }))
                .Distinct()
                .ToList();

            if (adjacent.Count == 0)
            {
                break;
            }
            
            layer = adjacent;
            layer.ForEach(pt => basin.Add(pt));
        }

        return basin.LongCount();
    }

    private static bool IsLower(int x, int y, int[,] data)
    {
        var lengthX = data.GetLength(0);
        var lengthY = data.GetLength(1);

        var adjacent = GetAdjacent(x, y).Where(p => p.X >= 0 && p.Y >= 0 && p.X < lengthX && p.Y < lengthY);
        var current = data[x, y];
        return adjacent.All(pt => data[pt.X, pt.Y] > current);
    }

    private static IEnumerable<Point> GetAdjacent(int sourceX, int sourceY)
    {
        yield return new Point(sourceX, sourceY + 1);
        yield return new Point(sourceX, sourceY - 1);
        yield return new Point(sourceX + 1, sourceY);
        yield return new Point(sourceX - 1, sourceY);
    }

    private static int[,] ParseInput()
    {
        var lines = Input.Split(Environment.NewLine);
        var result = new int[lines.Length, lines[0].Length];

        foreach (var i in Enumerable.Range(0, lines.Length))
        {
            var line = lines[i];
            foreach (var j in Enumerable.Range(0, line.Length))
            {
                result[i, j] = line[j] - '0';
            }
        }
        
        return result;
    }
}
