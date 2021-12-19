namespace AdventOfCode2021.Solutions;

public partial class Day14 : IDay
{
    private record struct GrowthPlace(char Left, char Right);
    
    private record struct GrowthRule(GrowthPlace Place, char Created);

    private record struct GrowthResult(GrowthPlace Place, long Count);
    
    /// <summary>
    /// For count 10 the simple algorithm works so let it stay here
    /// </summary>
    public long CalculatePartOne()
    {
        var (source, rules) = ParseInput();
        var rulesMap = rules.ToDictionary(r => r.Place, r => r.Created);

        var newSource = source;
        
        foreach (var i in Enumerable.Range(1, 10))
        {
            newSource = ApplyRules(newSource, rulesMap);
        }            

        var stats = newSource.GroupBy(ch => ch)
            .Select(ch => new { ch, count = ch.LongCount() })
            .Select(g => g.count)
            .OrderBy(k => k)
            .ToArray();

        return stats.Last() - stats.First();
    }
    
    /// <summary>
    /// Here I had to think of something more sophisticated.
    /// So instead of storing full arrays (which are very long) I stored pairs of adjacent letters and count of such pairs.
    /// At the end we sum up all chars counts and divide them by two (as each char except first and last will appear two times). 
    /// </summary>
    public long CalculatePartTwo()
    {
        var (source, rules) = ParseInput();
        var rulesMap = rules.ToDictionary(r => r.Place, r => r.Created);
        var distinctChars = source.Distinct().ToArray();
        
        var conversionResults = (from left in distinctChars
                                from right in distinctChars
                                let src = new GrowthPlace(left, right)
                                let result = ApplyRules(new[] { left, right }, rulesMap)
                                select new { src, result })
            .ToDictionary(
                d => d.src,
                d => d.result.Zip(d.result.Skip(1), (left, right) => new GrowthPlace(left, right)).ToArray());

        var sourcePairs = source.Zip(source.Skip(1), (left, right) => new GrowthPlace(left, right)).ToArray();

        var currentPairs = sourcePairs.GroupBy(p => p)
            .Select(grp => new GrowthResult(grp.Key, grp.LongCount()))
            .ToArray();
        
        foreach (var i in Enumerable.Range(1, 40))
        {
            currentPairs = currentPairs.SelectMany(
                p => conversionResults[p.Place].Select(pl => new GrowthResult(pl, p.Count))
                )
                .GroupBy(r => r.Place)
                .Select(grp =>
                    new GrowthResult(
                        grp.Key, 
                        grp.Select(p => p.Count).Sum()))
                .ToArray();
        }

        var resultChars = currentPairs.SelectMany(p => new[]
            {
                new { ch = p.Place.Left, count = p.Count },
                new { ch = p.Place.Right, count = p.Count }
            })
            .GroupBy(d => d.ch)
            .Select(grp => new { ch = grp.Key, count = grp.Select(g => g.count).Sum() })
            .ToDictionary(d => d.ch, d => d.count);

        resultChars[source.First()]++;
        resultChars[source.Last()]++;

        foreach (var k in resultChars.Keys.ToArray())
        {
            resultChars[k] /= 2;
        }

        var counts = resultChars.Select(p => p.Value).OrderBy(x => x).ToArray();

        return counts.Last() - counts.First();
    }

    private static char[] ApplyRules(char[] source, Dictionary<GrowthPlace, char> rules)
    {
        return source
            .Zip(source.Skip(1), (left, right) => new GrowthPlace(left, right))
            .SelectMany(place =>
            {
                if (rules.TryGetValue(place, out var newChar))
                {
                    return new[] { place.Left, newChar };
                }

                return new[] { place.Left };
            })
            .Concat(new[] { source.Last() })
            .ToArray();
    }

    private static (char[] source, GrowthRule[] rules) ParseInput()
    {
        var parts = Input.Split(Environment.NewLine + Environment.NewLine);
        var source = parts[0].ToCharArray();
        var rules = parts[1].Split(Environment.NewLine)
            .Select(line =>
            {
                var ruleParts = line.Split(" -> ");
                return new GrowthRule(new GrowthPlace(ruleParts[0][0], ruleParts[0][1]), ruleParts[1][0]);
            })
            .ToArray();

        return (source, rules);
    }
}
