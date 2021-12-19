namespace AdventOfCode2021.Solutions;

public sealed partial class Day11 : IDay
{
    private record struct Point(int X, int Y)
    {
        public override string ToString()
        {
            return $"x={X},y={Y}";
        }
    }
    
    public long CalculatePartOne()
    {
        var data = ParseInput();
        var result = 0L;
        foreach (var i in Enumerable.Range(1, 100))
        {
            result += CalculateNextStep(data);
        }

        return result;
    }

    public long CalculatePartTwo()
    {
        var data = ParseInput();
        var step = 0;
        var maxFlashes = data.GetLength(0) * data.GetLength(1);
        while (true)
        {
            step++;
            var flashes = CalculateNextStep(data);
            if (flashes == maxFlashes)
            {
                return step;
            }
        }
    }

    private static int CalculateNextStep(int[,] data)
    {
        var lengthX = data.GetLength(0);
        var lengthY = data.GetLength(1);

        var indices = new[] { -1, 0, 1 };

        var flashesCount = 0;

        IEnumerable<Point> GetAdjacent(Point point)
        {
            return (
                from x in indices
                from y in indices
                where x != 0 || y != 0
                select new Point(point.X + x, point.Y + y)
            ).Where(p => p.X >= 0 && p.Y >= 0 && p.X < lengthX && p.Y < lengthY);
        }

        var flashes = new HashSet<Point>();
        foreach (var x in Enumerable.Range(0, data.GetLength(0)))
        {
            foreach (var y in Enumerable.Range(0, data.GetLength(1)))
            {
                data[x, y]++;
                if (data[x, y] > 9)
                {
                    data[x, y] = 0;
                    var point = new Point(x, y);
                    flashes.Add(point);
                    flashesCount++;
                    
                }
            }
        }

        var flashesLevel = flashes.ToList();
        while (true)
        {
            var newLevel = new List<Point>();
            foreach (var point in flashesLevel)
            {
                foreach (var adjacent in GetAdjacent(point).Where(pt => !flashes.Contains(pt)))
                {
                    data[adjacent.X, adjacent.Y]++;
                    if (data[adjacent.X, adjacent.Y] > 9)
                    {
                        data[adjacent.X, adjacent.Y] = 0;
                        newLevel.Add(adjacent);
                        flashes.Add(adjacent);
                        flashesCount++;
                    }
                }
            }

            if (newLevel.Count == 0)
            {
                break;
            }

            flashesLevel = newLevel;
        }

        return flashesCount;
    }

    private static void Print(int[,] data)
    {
        foreach (var x in Enumerable.Range(0, data.GetLength(0)))
        {
            foreach (var y in Enumerable.Range(0, data.GetLength(1)))
            {
                Console.Write(data[x,y]);
            }
            Console.WriteLine();
        }
    }

    private static int[,] ParseInput()
    {
        var data = Input.Split(Environment.NewLine);
        var result = new int[data.Length, data[0].Length];

        foreach (var x in Enumerable.Range(0, data.Length))
        {
            foreach (var y in Enumerable.Range(0, data[0].Length))
            {
                result[x, y] = int.Parse(data[x][y].ToString());
            }
        }

        return result;
    }
}
