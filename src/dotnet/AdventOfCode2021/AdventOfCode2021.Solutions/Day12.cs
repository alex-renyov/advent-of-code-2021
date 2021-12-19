using System.Linq;

namespace AdventOfCode2021.Solutions;

public partial class Day12 : IDay
{
    private record Way(string From, string To);
    
    public int DayNumber => 12;
    
    public long CalculatePartOne()
    {
        return Calculate(CanAddTo1);
    }

    public long CalculatePartTwo()
    {
        return Calculate(CanAddTo2);
    }

    private static long Calculate(Func<IReadOnlyList<string>, string, bool> canAdd)
    {
        var input = ParseInput();
        var ways = new IReadOnlyList<string>[] { new[] { "start" } };
        while (true)
        {
            var newWays = new List<IReadOnlyList<string>>();
            var newWaysAdded = false;

            foreach (var way in ways)
            {
                if (way.Last() == "end")
                {
                    newWays.Add(way);
                    continue;
                }

                if (input.TryGetValue(way.Last(), out var connections))
                {
                    foreach (var next in connections)
                    {
                        if (canAdd(way, next))
                        {
                            newWaysAdded = true;
                            newWays.Add(way.Concat(new[] { next }).ToArray());
                        }
                    }
                }
            }

            ways = newWays.ToArray();

            if (!newWaysAdded)
            {
                break;
            }
        }
        
        return ways.Length;
    }

    private static void Print(IReadOnlyList<string>[] ways)
    {
        foreach (var w in ways)
        {
            Console.WriteLine(string.Join(",", w));
        }
    }

    private static bool CanAddTo1(IReadOnlyList<string> way, string newSegment)
    {
        return newSegment.ToCharArray().All(char.IsUpper) || !way.Contains(newSegment);
    }
    
    private static bool CanAddTo2(IReadOnlyList<string> way, string newSegment)
    {
        if (newSegment == "start")
        {
            return false;
        }

        if (newSegment.ToCharArray().All(char.IsUpper))
        {
            return true;
        }

        var newSegmentCount = way.Count(testStr => testStr == newSegment);
        if (newSegmentCount == 0)
        {
            return true;
        }
        if (newSegmentCount >= 2)
        {
            return false;
        }

        return !way.Where(way => way.ToCharArray().All(char.IsLower)).GroupBy(str => str).Any(grp => grp.Count() > 1);
    }

    private static Dictionary<string, string[]> ParseInput()
    {
        var result = Input.Split(Environment.NewLine)
            .SelectMany(line =>
            {
                var parts = line.Split("-").ToArray();
                return new[]
                {
                    new Way(parts[0], parts[1]),
                    new Way(parts[1], parts[0])
                };
            }).ToArray();
        
        return result.GroupBy(w => w.From).ToDictionary(grp => grp.Key, grp => grp.Select(w => w.To).ToArray());
    }
}
