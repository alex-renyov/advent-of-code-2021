namespace AdventOfCode2021.Solutions;

public partial class Day7 : IDay
{
    private record struct Crab(int Position, int Count);

    private record struct Travel(Crab Crab, int Distance);

    private record struct Result(int POsition, long TotalFuel);

    public int DayNumber => 7;

    public long CalculatePartOne()
    {
        return CalculateMinimumFuel(x => x);
    }

    public long CalculatePartTwo()
    {
        return CalculateMinimumFuel(x => (x + 1) * x / 2);
    }

    private static long CalculateMinimumFuel(Func<int, int> fuelCalculator)
    {
        var input = ParseInput();

        var minPosition = input.Min();
        var maxPosition = input.Max();

        var crabs = input.GroupBy(x => x).Select(x => new Crab(x.Key, x.Count())).ToArray();

        var result = Enumerable.Range(minPosition, maxPosition - minPosition + 1)
            .Select(pos =>
            {
                var fuel = crabs.Select(cr => new Travel(cr, Math.Abs(cr.Position - pos)))
                            .Aggregate(0L, (fuel, crabs) => fuel + fuelCalculator(crabs.Distance) * crabs.Crab.Count);
                return new Result(pos, fuel);
            }).OrderBy(r => r.TotalFuel).First();

        return result.TotalFuel;
    }

    private static int[] ParseInput()
    {
        return Input.Split(",").Select(int.Parse).ToArray();
    }
}
