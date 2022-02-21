using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AdventOfCode2021.Solutions;

public partial class Day17 : IDay
{
    private record struct Range(int From, int To);

    public long CalculatePartOne()
    {
        var (horizontal, vertical) = ParseInput();

        int maxHeight = 0;

        var verticalLimits = new List<(int StartSpeed, int MaxHeight, int[] Times)>();
        foreach (var startVertical in Enumerable.Range(1, -vertical.From))
        {
            var verticals = GetVerticalCoords(startVertical).TakeWhile(v => v.position >= vertical.From)
                .ToArray();
            var currentMaxHeight = verticals.Max(v => v.position);
            var times = verticals.Where(v => v.position <= vertical.To).Select(v => v.time).ToArray();
            if (!times.Any())
            {
                continue;
            }

            verticalLimits.Add((startVertical, currentMaxHeight, times));
        }

        var sortedLimits = verticalLimits.OrderByDescending(l => l.MaxHeight).ToArray();

        int startHorizontal = 0;

        while (true)
        {
            startHorizontal++;
            if (startHorizontal > horizontal.To)
            {
                break;
            }

            var horizontals = GetHorizontalCoords(startHorizontal)
                .Where(p => p.position >= horizontal.From && p.position <= horizontal.To)
                .ToArray();

            if (!horizontals.Any())
            {
                continue;
            }

            var times = horizontals.Select(h => h.time).ToArray();
            var currentMax = sortedLimits.Where(l => l.Times.Intersect(times).Any())
                .Select(l => (int?)l.MaxHeight)
                .FirstOrDefault();
            if (currentMax.HasValue)
            {
                maxHeight = Math.Max(maxHeight, currentMax.Value);
            }
        }
        
        return maxHeight;
    }

    private static IEnumerable<(int position, int time)> GetHorizontalCoords(int startSpeed)
    {
        var speed = startSpeed;
        var position = 0;
        var time = 0;

        do
        {
            position += speed;
            time++;
            speed = speed switch
            {
                > 0 => speed - 1,
                < 0 => speed + 1,
                _ => speed
            };

            yield return (position, time);
        } while (speed != 0);

        foreach (var i in Enumerable.Range(0, 1000))
        {
            time++;
            yield return (position, time);
        }
    }

    private static IEnumerable<(int position, int time)> GetVerticalCoords(int startSpeed)
    {
        var speed = startSpeed;
        var position = 0;
        var time = 0;
        while (true)
        {
            position += speed;
            time++;
            speed--;

            yield return (position, time);
        }
    }

    public long CalculatePartTwo()
    {
        var (horizontal, vertical) = ParseInput();

        var verticalLimits = new List<(int StartSpeed, HashSet<int> Times)>();
        foreach (var startVertical in Enumerable.Range(vertical.From * 2, -vertical.From * 4))
        {
            var verticals = GetVerticalCoords(startVertical).TakeWhile(v => v.position >= vertical.From)
                .ToArray();
            var times = verticals.Where(v => v.position <= vertical.To).Select(v => v.time).ToArray();
            if (!times.Any())
            {
                continue;
            }

            verticalLimits.Add((startVertical, times.ToHashSet()));
        }

        var horizontalLimits = new List<(int StartSpeed, HashSet<int> Times)>();
        foreach (var startHorizontal in Enumerable.Range(-horizontal.To * 2, horizontal.To * 4))
        {
            var horizontals = GetHorizontalCoords(startHorizontal).TakeWhile(v => v.position <= horizontal.To)
                .ToArray();
            var times = horizontals.Where(v => v.position >= horizontal.From).Select(v => v.time).ToArray();
            if (!times.Any())
            {
                continue;
            }

            horizontalLimits.Add((startHorizontal, times.ToHashSet()));
        }

        return (from hor in horizontalLimits
            from vert in verticalLimits
            where vert.Times.Overlaps(hor.Times)
            select new { HorStart = hor.StartSpeed, VertStart = vert.StartSpeed }).Count();
    }

    private static (Range horizontal, Range Vertical) ParseInput()
    {
        var match = Regex.Match(Input, @"target area: x=(\-?\d+)..(\-?\d+), y=(\-?\d+)..(\-?\d+)");
        var horizontal = new Range(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        var vertical = new Range(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

        return (horizontal, vertical);
    }
}
