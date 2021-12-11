namespace AdventOfCode2021.Solutions;

public sealed partial class Day10 : IDay
{
    public int DayNumber => 10;

    private static string[] Parsed => Input.Split(Environment.NewLine);

    private static readonly Dictionary<char, int> InvalidCharScore = new()
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137
    };
    
    private static readonly Dictionary<char, int> MissingCharScore = new()
    {
        [')'] = 1,
        [']'] = 2,
        ['}'] = 3,
        ['>'] = 4
    };

    private static readonly Dictionary<char, char> SymbolPairs = new()
    {
        ['('] = ')',
        ['['] = ']',
        ['{'] = '}',
        ['<'] = '>',
    };
    
    public long CalculatePartOne()
    {
        return Parsed.Select(GetInvalidLineScore).Aggregate(0L, (x, y) => x + y);
    }
    
    public long CalculatePartTwo()
    {
        var scores = Parsed.Select(GetIncompleteLineScore).Where(s => s != 0).OrderBy(s => s).ToArray();
        return scores[scores.Length / 2];
    }

    private static int GetInvalidLineScore(string line)
    {
        var closedWaiting = new Stack<char>();
        foreach (var ch in line)
        {
            if (SymbolPairs.TryGetValue(ch, out var closingChar))
            {
                closedWaiting.Push(closingChar);
            }
            else
            {
                if (!closedWaiting.TryPeek(out var foundChar) || foundChar != ch)
                {
                    return InvalidCharScore[ch];
                }

                closedWaiting.Pop();
            }
        }
        return 0;
    }
    
    private static long GetIncompleteLineScore(string line)
    {
        var closedWaiting = new Stack<char>();
        foreach (var ch in line)
        {
            if (SymbolPairs.TryGetValue(ch, out var closingChar))
            {
                closedWaiting.Push(closingChar);
            }
            else
            {
                if (!closedWaiting.TryPeek(out var foundChar) || foundChar != ch)
                {
                    return 0;
                }

                closedWaiting.Pop();
            }
        }

        var result = 0L;

        while (closedWaiting.TryPop(out var remain))
        {
            result = result * 5 + MissingCharScore[remain];
        }

        return result;
    }
}
