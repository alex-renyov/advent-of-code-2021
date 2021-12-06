
namespace AdventOfCode2021.Solutions;

public partial class Day1 : IDay
{
    public long CalculatePartOne()
    {
        var values = ParseValues();
        return values.Skip(1)
          .Zip(values, (next, prev) => next > prev)
          .Where(val => val)
          .Count();
    }

    public long CalculatePartTwo()
    {
        static int Add(int a, int b) => a + b;

        var values = ParseValues();
        var sumTriples = values.Skip(2)
          .Zip(values.Skip(1), Add)
          .Zip(values, Add)
          .ToArray();

        return sumTriples.Skip(1)
          .Zip(sumTriples, (next, prev) => next > prev)
          .Where(val => val)
          .Count();
    }

    public int DayNumber => 1;

    private static int[] ParseValues()
    {
        return Input.Split(Environment.NewLine)
          .Select(int.Parse)
          .ToArray();
    }
}
