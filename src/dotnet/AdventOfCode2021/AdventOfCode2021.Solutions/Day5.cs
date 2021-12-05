namespace AdventOfCode2021.Solutions;

public partial class Day5
{
  private record struct Coords(int X, int Y);

  private record struct Line(Coords From, Coords To);

  public int CalculatePartOne()
  {
    Console.WriteLine("Day 5. Part 1");

    var lines = ParseInput();
    var withoutDiagonal = lines.Where(l => l.From.X == l.To.X || l.From.Y == l.To.Y)
      .ToArray();

    var field = new Dictionary<Coords, int>();
    FillField(withoutDiagonal, field);

    return field.Where(f => f.Value > 1).Count();
  }

  public int CalculatePartTwo()
  {
    Console.WriteLine("Day 5. Part 2");

    var lines = ParseInput();

    var field = new Dictionary<Coords, int>();
    FillField(lines, field);

    return field.Where(f => f.Value > 1).Count();
  }

  private void Print(Dictionary<Coords, int> field)
  {
    var maxX = field.Keys.Select(k => k.X).OrderByDescending(a => a).First();
    var maxY = field.Keys.Select(k => k.Y).OrderByDescending(a => a).First();

    foreach (var x in Enumerable.Range(0, maxX + 1))
    {
      var parts = Enumerable.Range(0, maxY + 1)
        .Select(y => field.GetValueOrDefault(new Coords(y, x)))
        .Select(v => v == 0 ? "." : v.ToString());
      Console.WriteLine(string.Join(string.Empty, parts));
    }
  }

  private void FillField(IEnumerable<Line> lines, Dictionary<Coords, int> field)
  {
    foreach (var line in lines)
    {
      if (line.From.X == line.To.X)
      {
        foreach (var y in MakeSequence(line.From.Y, line.To.Y))
        {
          var coord = new Coords(line.From.X, y);
          field[coord] = field.GetValueOrDefault(coord) + 1;
        }
      }
      else if (line.From.Y == line.To.Y)
      {
        foreach (var x in MakeSequence(line.From.X, line.To.X))
        {
          var coord = new Coords(x, line.From.Y);
          field[coord] = field.GetValueOrDefault(coord) + 1;
        }
      }
      else
      {
        foreach (var point in GetLinePoints(line))
        {
          field[point] = field.GetValueOrDefault(point) + 1;
        }
      }
    }
  }

  private static IEnumerable<Coords> GetLinePoints(Line line)
  {
    var xs = MakeSequence(line.From.X, line.To.X);
    var ys = MakeSequence(line.From.Y, line.To.Y);

    return xs.Zip(ys, (x, y) => new Coords(x, y));
  }

  private static IEnumerable<int> MakeSequence(int from, int to)
  {
    var diff = from < to ? 1 : -1;
    var length = Math.Abs(to - from) + 1;

    return Enumerable.Range(0, length).Select(i => from + i * diff);
  }

  private static Line[] ParseInput()
  {
    return Input.Split(Environment.NewLine)
      .Select(
        line => line.Split(" -> ")
          .Select(pair => pair.Split(",")
            .Select(int.Parse)
            .ToArray())
          .Select(p => new Coords(p[0], p[1]))
          .ToArray())
      .Select(c => new Line(c[0], c[1]))
      .ToArray();
  }
}