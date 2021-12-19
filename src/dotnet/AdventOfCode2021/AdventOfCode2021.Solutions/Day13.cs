namespace AdventOfCode2021.Solutions;

public partial class Day13 : IDay
{
    private record struct Point(int X, int Y);

    private record struct Fold(int? X = null, int? Y = null);
    
    public long CalculatePartOne()
    {
        var (points, folds) = ParseInput();
        var afterFirstFold = PerformFold(points, folds[0]);
        return afterFirstFold.LongCount();
    }

    // We got BLHFJPJF
    //  OOO..O....O..O.OOOO...OO.OOO....OO.OOOO
    //  O..O.O....O..O.O.......O.O..O....O.O...
    //  OOO..O....OOOO.OOO.....O.O..O....O.OOO.
    //  O..O.O....O..O.O.......O.OOO.....O.O...
    //  O..O.O....O..O.O....O..O.O....O..O.O...
    //  OOO..OOOO.O..O.O.....OO..O.....OO..O...
    public long CalculatePartTwo()
    {
        var (points, folds) = ParseInput();
        var final = folds.Aggregate(points, PerformFold);
        Print(final);
        return 0;
    }

    private static Point[] PerformFold(Point[] points, Fold fold)
    {
        return points.Select(pt =>
        {
            if (fold.X is { } foldX)
            {
                if (pt.X < foldX)
                {
                    return pt;
                }

                return pt with { X = foldX * 2 - pt.X };
            }

            if (fold.Y is { } foldY)
            {
                if (pt.Y < foldY)
                {
                    return pt;
                }

                return pt with { Y = foldY * 2 - pt.Y };
            }

            throw new InvalidOperationException("Empty fold");
        }).Distinct().ToArray();
    }

    private static void Print(Point[] points)
    {
        var maxX = points.Select(pt => pt.X).Max();
        var maxY = points.Select(pt => pt.Y).Max();

        var lines = points.GroupBy(pt => pt.Y).ToDictionary(grp => grp.Key, grp => grp.Select(p => p.X).ToHashSet());
        foreach (var y in Enumerable.Range(0, maxY + 1))
        {
            if (lines.TryGetValue(y, out var allX))
            {
                var line = Enumerable.Range(0, maxX + 1)
                    .Select(x => allX.Contains(x) ? 'O' : '.')
                    .ToArray();
                Console.WriteLine(new string(line));
            }
            else
            {
                Console.WriteLine(new string(Enumerable.Repeat('.', maxX).ToArray()));
            }
        }
    }

    private static (Point[] points, Fold[] folds) ParseInput()
    {
        var mainParts = Input.Split(Environment.NewLine + Environment.NewLine);

        var points = mainParts[0].Split(Environment.NewLine).Select(line =>
        {
            var parts = line.Split(",").Select(int.Parse).ToArray();
            return new Point(parts[0], parts[1]);
        }).ToArray();

        var folds = mainParts[1].Split(Environment.NewLine).Select(line =>
        {
            var data = line.Substring("fold along ".Length);
            var parts = data.Split("=");
            var value = int.Parse(parts[1]);
            return parts[0] == "x" ? new Fold(X: value) : new Fold(Y: value);
        }).ToArray();

        return (points, folds);
    }
}
