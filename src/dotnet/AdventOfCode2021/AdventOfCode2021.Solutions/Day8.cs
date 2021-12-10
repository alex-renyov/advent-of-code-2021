namespace AdventOfCode2021.Solutions;

public partial class Day8 : IDay
{
    private record struct InputLine(string[] Patterns, string[] Values);
    
    private static readonly Dictionary<int, string> NumberDigits = new()
    {
        [0] = "abcefg",
        [1] = "cf",
        [2] = "acdeg",
        [3] = "acdfg",
        [4] = "bcdf",
        [5] = "abdfg",
        [6] = "abdefg",
        [7] = "acf",
        [8] = "abcdefg",
        [9] = "abcdfg"
    };

    private static readonly Dictionary<string, int> ReversedNumberDigits =
        NumberDigits.ToDictionary(p => p.Value, p => p.Key);

    public int DayNumber => 8;
    
    public long CalculatePartOne()
    {
        var input = ParseInput();

        var simpleLengths = new [] { 2, 3, 4, 7 };

        return input.SelectMany(i => i.Values).Select(v => v.Length).Count(l => simpleLengths.Contains(l));
    }

    public long CalculatePartTwo()
    {
        var charMapping = DescribeInput(NumberDigits.Values);
        var reversedMapping = charMapping.ToDictionary(p => p.Value, p => p.Key);
        
        var input = ParseInput();
        long sum = 0;

        foreach (var line in input)
        {
            var map = CalculateMapping(line.Patterns, reversedMapping);
            var parts = line.Values.Select(v => map[OrderChars(v)]).ToArray();
            var value = int.Parse(string.Join(string.Empty, parts));
            sum += value;
        }

        return sum;
    }

    private static Dictionary<string, int> CalculateMapping(string[] input, Dictionary<string, char> reversedCharMapping)
    {
        var description = DescribeInput(input);
        var newToOldMapping = description.Keys.ToDictionary(ch => ch, ch => reversedCharMapping[description[ch]]);
        
        return input.ToDictionary(OrderChars, s =>
        {
            var mapped = s.ToCharArray().Select(ch => newToOldMapping[ch]).OrderBy(ch => ch).ToArray();
            return ReversedNumberDigits[new string(mapped)];
        });
    }

    /// <summary>
    /// Main idea here is to describe each char as lengths of digits that it is contained in.
    /// For example e is in 0 (length 6), 2 (length 5), 6 (length 6) and 8 (length 7) so we match e with string 5 6 6 7 (non decreasing order)
    /// For each letter we have unique string, so by matching this hash codes we can easily find corresponding mapping
    /// </summary>
    private static Dictionary<char, string> DescribeInput(IEnumerable<string> input)
    {
        return input.SelectMany(s => s.ToCharArray().Select(ch => new { ch, lng = s.Length }))
            .GroupBy(v => v.ch)
            .Select(grp => new { ch = grp.Key, lengths = string.Join(",", grp.Select(v => v.lng).OrderBy(i => i)) })
            .ToDictionary(g => g.ch, g => g.lengths);
    }

    private static string OrderChars(string source)
    {
        return new string(source.ToCharArray().OrderBy(ch => ch).ToArray());
    }
    
    private static InputLine[] ParseInput()
    {
        return Input.Split(Environment.NewLine)
            .Select(line =>
            {
                var parts = line.Split(" | ");
                return new InputLine(
                    parts[0].Trim().Split(" "),
                    parts[1].Trim().Split(" ")
                );
            }).ToArray();
    }
}
