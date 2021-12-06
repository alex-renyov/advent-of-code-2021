namespace AdventOfCode2021.Solutions;

public partial class Day6 : IDay
{
    private record class LanternsGroup(int Timer, long Count)
    {
        public IEnumerable<LanternsGroup> GetNextGroup()
        {
            if (Timer != 0)
            {
                yield return new LanternsGroup(Timer - 1, Count);
                yield break;
            }

            yield return new LanternsGroup(6, Count);
            yield return new LanternsGroup(8, Count);
        }
    }

    public long CalculatePartOne()
    {
        return CalculateCount(80);
    }

    public long CalculatePartTwo()
    {
        return CalculateCount(256);
    }

    public int DayNumber => 6;

    private static long CalculateCount(int days)
    {
        var lanterns = ParseInput().GroupBy(val => val)
            .Select(grp => new LanternsGroup(grp.Key, grp.Count()))
            .ToArray();

        foreach (var i in Enumerable.Range(0, days))
        {
            lanterns = lanterns.SelectMany(l => l.GetNextGroup())
                .GroupBy(g => g.Timer)
                .Select(grp => new LanternsGroup(
                    grp.Key,
                    grp.Select(g => g.Count).Aggregate((v1, v2) => v1 + v2))
                    )
                .ToArray();
        }

        return lanterns.Select(l => l.Count).Aggregate((v1, v2) => v1 + v2);
    }

    private static int[] ParseInput()
    {
        return Input.Split(",").Select(int.Parse).ToArray();
    }
}
