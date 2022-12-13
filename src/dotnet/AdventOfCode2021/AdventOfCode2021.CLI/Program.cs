using System.Diagnostics;
using AdventOfCode2021.Solutions;

var dayInterface = typeof(IDay);
var types = dayInterface.Assembly.GetTypes().Where(t => dayInterface.IsAssignableFrom(t) && !t.IsAbstract);
var instances = types.Select(t => Activator.CreateInstance(t) as IDay ?? throw new InvalidOperationException($"Invalid type {t.FullName}"))
    .OrderBy(d => d.DayNumber)
    .ToArray();

foreach (var day in instances.Where(i => i.DayNumber == 19))
{
    var sw = Stopwatch.StartNew();
    Console.WriteLine($"Day {day.DayNumber} part 1");
    Console.WriteLine(day.CalculatePartOne());
    Console.WriteLine($"Day {day.DayNumber} part 2");
    Console.WriteLine(day.CalculatePartTwo());
    sw.Stop();
    Console.WriteLine($"{sw.Elapsed} elapsed");
}
