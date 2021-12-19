namespace AdventOfCode2021.Solutions;

public partial class Day3 : IDay
{
    public long CalculatePartOne()
    {
        var lines = Input.Split(Environment.NewLine);
        var length = lines[0].Length;

        var gamma = new char[length];

        foreach (var i in Enumerable.Range(0, length))
        {
            var quantities = lines.Select(line => line[i])
              .GroupBy(ch => ch)
              .Select(ch => new { ch = ch.Key, count = ch.Count() })
              .ToDictionary(c => c.ch, c => c.count);

            gamma[i] = quantities['1'] >= quantities['0'] ? '1' : '0';
        }

        var epsilon = gamma.Select(ch => ch == '1' ? '0' : '1').ToArray();

        var gammaValue = Convert.ToInt32(new string(gamma), 2);
        var epsilonValue = Convert.ToInt32(new string(epsilon), 2);

        return gammaValue * epsilonValue;
    }

    public long CalculatePartTwo()
    {
        var lines = Input.Split(Environment.NewLine);
        var length = lines[0].Length;

        int CalculateNumber(string[] source, Func<(int count0, int count1), char> charComparison)
        {
            var currentLines = source;
            foreach (var i in Enumerable.Range(0, length))
            {
                var counts = currentLines.Select(line => line[i])
                  .GroupBy(ch => ch)
                  .ToDictionary(grp => grp.Key, grp => grp.Count());
                var foundChar = charComparison((counts['0'], counts['1']));
                currentLines = currentLines.Where(line => line[i] == foundChar).ToArray();

                if (currentLines.Length == 1)
                {
                    return Convert.ToInt32(currentLines[0], 2);
                }
            }

            throw new Exception("Something went wrong");
        }

        var oxygen = CalculateNumber(lines, counts => counts.count1 >= counts.count0 ? '1' : '0');
        var co2 = CalculateNumber(lines, counts => counts.count0 <= counts.count1 ? '0' : '1');

        return oxygen * co2;
    }
}
