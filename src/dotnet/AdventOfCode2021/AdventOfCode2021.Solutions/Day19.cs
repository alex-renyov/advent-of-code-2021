namespace AdventOfCode2021.Solutions;

public partial class Day19 : IDay
{
    private record struct Point(int X, int Y, int Z);
    private record struct Orientation(short X, short Y, short Z);
    private const int OverlapThreshold = 12;
    
    public Int64 CalculatePartOne()
    {
        ParseInput();
        return 0;
    }

    public Int64 CalculatePartTwo()
    {
        return 0;
    }

    private static bool TryOverlap(Point[] source, Point[] target, out Point position, out Orientation orientation)
    {
        position = default;
        orientation = default;

        foreach (var testOrientation in AllOrientations())
        {
            
        }
    }

    private static IEnumerable<Orientation> AllOrientations()
    {
        var signs = new short[] { 1, -1 };
        return from x in signs
            from y in signs
            from z in signs
            select new Orientation(x, y, z);
    }

    private Point[][] ParseInput()
    {
        List<Point[]> result = new();
        List<Point> currentSet = new();

        foreach (var line in TestInput.Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line))
            {
                result.Add(currentSet.ToArray());
                currentSet = new();
                continue;
            }

            if (line.Contains("scanner"))
            {
                continue;
            }

            var coords = line.Split(",").Select(int.Parse).ToArray();
            currentSet.Add(new Point(coords[0], coords[1], coords[2]));
        }

        if (currentSet.Any())
        {
            result.Add(currentSet.ToArray());
        }

        return result.ToArray();
    }
}
