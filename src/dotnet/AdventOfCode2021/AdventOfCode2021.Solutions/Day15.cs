namespace AdventOfCode2021.Solutions;

public partial class Day15 : IDay
{
    private record struct Point(int X, int Y);
    
    public long CalculatePartOne()
    {
        var input = ParseInput();
        var length = input.GetLength(0);

        var dangerLevels = new Dictionary<Point, long>
        {
            [new Point(0, 0)] = 0
        };

        foreach (var distance in Enumerable.Range(1, 2 * (length - 1)))
        {
            foreach (var point in GetPoints(distance, length))
            {
                dangerLevels[point] = CalculateDangerLevel(point, dangerLevels, input);
            }
        }
        
        return dangerLevels[new Point(length - 1, length - 1)];
    }
    
    public long CalculatePartTwo()
    {
        var input = ExtendInput(ParseInput(), 5);
        var length = input.GetLength(0);

        var dangerLevels = new Dictionary<Point, long>
        {
            [new Point(0, 0)] = 0
        };

        foreach (var distance in Enumerable.Range(1, 2 * (length - 1)))
        {
            foreach (var point in GetPoints(distance, length).AsParallel().WithDegreeOfParallelism(8))
            {
                dangerLevels[point] = CalculateDangerLevel(point, dangerLevels, input);
            }
        }
        
        return dangerLevels[new Point(length - 1, length - 1)];
    }

    private long CalculateDangerLevel(Point point, IDictionary<Point, long> dangerLevels, int[,] input)
    {
        var simpleLevel = point switch
        {
            { X: 0 } => dangerLevels[point with { Y = point.Y - 1}],
            { Y: 0 } => dangerLevels[point with { X = point.X - 1}],
            _ => Math.Min(dangerLevels[point with { Y = point.Y - 1}], dangerLevels[point with { X = point.X - 1}])
        } + input[point.X, point.Y];

        var length = input.GetLength(0);
        var distance = point.X + point.Y;

        var currentStep = new[] { new { pt = point, level = (long)input[point.X, point.Y] } }.ToList();
        var currentLevel = simpleLevel;
        var minDiagonal = GetPoints(distance - 1, length).Select(pt => dangerLevels[pt]).Min();

        var processedPoints = new HashSet<Point> { point };

        while (currentStep.Any())
        {
            var newPositions = currentStep.SelectMany(pos =>
            {
                var currentLevel = pos.level;
                var newPoints = new[]
                {
                    pos.pt with { X = pos.pt.X + 1 },
                    pos.pt with { X = pos.pt.X - 1 },
                    pos.pt with { Y = pos.pt.Y + 1 },
                    pos.pt with { Y = pos.pt.Y - 1 }
                }.Where(p => p.X >= 0 && p.Y >= 0 && p.X < length && p.Y < length && !processedPoints.Contains(p));

                var newLevels = newPoints.Select(pt => new { pt, level = currentLevel + input[pt.X, pt.Y] });
                return newLevels;
            })
                .GroupBy(pos => pos.pt)
                .Select(grp => new { pt = grp.Key, level = grp.Select(pos => pos.level).Min() })
                .ToList();
            
            newPositions.ForEach(pos => processedPoints.Add(pos.pt));

            var knownLevels = newPositions.Where(pos => dangerLevels.ContainsKey(pos.pt));
            foreach (var known in knownLevels)
            {
                var calculatedLevel = dangerLevels[known.pt] + known.level - input[known.pt.X, known.pt.Y];
                currentLevel = Math.Min(calculatedLevel, currentLevel);
            }
            
            currentStep = newPositions
                .Where(pos => !dangerLevels.ContainsKey(pos.pt))
                .Where(pos => pos.level + minDiagonal + (pos.pt.X + pos.pt.Y - distance) < currentLevel).ToList();
        }

        return currentLevel;
    }

    private static IEnumerable<Point> GetPoints(int distance, int length)
    {
        return (from x in Enumerable.Range(0, length)
            let y = distance - x
            where y >= 0 && y < length
            select new Point(x, y));
    }

    private static int[,] ParseInput()
    {
        var lines = Input.Split(Environment.NewLine);
        var result = new int[lines.Length, lines.Length];
        var length = lines.Length;

        foreach (var x in Enumerable.Range(0, length))
        {
            foreach (var y in Enumerable.Range(0, length))
            {
                result[x, y] = int.Parse(lines[x][y].ToString());
            }
        }

        return result;
    }

    private static int[,] ExtendInput(int[,] input, int count)
    {
        var sourceLength = input.GetLength(0);
        var length = sourceLength * count;
        var result = new int[length, length];

        foreach (var x in Enumerable.Range(0, sourceLength))
        {
            foreach (var y in Enumerable.Range(0, sourceLength))
            {
                var value = input[x, y];

                foreach (var shiftX in Enumerable.Range(0, count))
                {
                    foreach (var shiftY in Enumerable.Range(0, count))
                    {
                        var shift = (shiftX + shiftY) % sourceLength;
                        result[x + shiftX * sourceLength, y + shiftY * sourceLength] = (value + shift - 1) % 9 + 1;
                    }
                }
            }
        }

        return result;
    }
}
