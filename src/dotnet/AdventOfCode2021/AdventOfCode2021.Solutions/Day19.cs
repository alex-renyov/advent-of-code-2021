namespace AdventOfCode2021.Solutions;

using FParsec.CSharp;
using static FParsec.CSharp.PrimitivesCS;
using static FParsec.CSharp.CharParsersCS;
using System.Linq;

public partial class Day19 : IDay
{
    private readonly record struct Point(int X, int Y, int Z)
    {
        public int this[int index] =>
            index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };

        public static Point operator +(in Point a, in Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static Point operator -(in Point a, in Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point operator *(in Point a, int[,] matrix)
        {
            var result = new int[3];

            foreach (var i in Enumerable.Range(0, 3))
            {
                result[i] = 0;
                foreach (var j in Enumerable.Range(0, 3))
                {
                    result[i] += a[j] * matrix[i, j];
                }
            }

            return new Point(result[0], result[1], result[2]);
        }

        public int ManhattanDistanceFrom(in Point target)
        {
            return Math.Abs(target.X - X) + Math.Abs(target.Y - Y) + Math.Abs(target.Z - Z);
        }
    }
    private sealed class Sensor
    {
        public Sensor(int Index, Point[] Points)
        {
            this.Index = Index;
            this.Points = Points;
        }

        public int Index { get; set; }
        public Point[] Points { get; set; }
        
        public Point? AbsoluteCoords { get; set; }
        
        public Orientation? AbsoluteOrientation { get; set; }
    }

    private readonly record struct Orientation(int[,] Transform)
    {
        public static Orientation Identity { get; } = MakeIdentity();

        private static Orientation MakeIdentity()
        {
            var transform = new int[3, 3];
            var reverse = new int[3, 3];

            foreach (var i in Enumerable.Range(0, 3))
            {
                transform[i, i] = reverse[i, i] = 1;
            }

            return new Orientation(transform);
        }
    } 

    private const int OverlapThreshold = 12;
    private static readonly Orientation[] Orientations = GetOrientations().ToArray();
    private Sensor[] sensors;

    public Int64 CalculatePartOne()
    {
        sensors = ParseInput(Input);
        var firstSensor = sensors.First();
        firstSensor.AbsoluteCoords = new Point(0, 0, 0);
        firstSensor.AbsoluteOrientation = Orientation.Identity;

        var points = firstSensor.Points.ToHashSet();

        Console.Write(new string(Enumerable.Repeat('_', sensors.Count() - 1).ToArray()));
        Console.Write("\r");

        while (true)
        {
            var changed = false;
            foreach (var sensor in sensors.Where(s => s.AbsoluteCoords is null))
            {
                var intersection = GetIntersection(points, sensor);
                if (intersection is {} intersectionValue)
                {
                    sensor.AbsoluteCoords = intersectionValue.relative;
                    sensor.AbsoluteOrientation = intersectionValue.orientation;
                    Console.Write("*");
                    intersectionValue.convertedSensorPoints.ForEach(p => points.Add(p));
                    changed = true;
                }
            }

            if (sensors.All(s => s.AbsoluteCoords is { }))
            {
                Console.WriteLine();
                Console.WriteLine();
                break;
            }

            if (!changed)
            {
                throw new InvalidOperationException("Dead end");
            }
        }

        return points.Count;
    }

    private static (Point relative, Orientation orientation, List<Point> convertedSensorPoints)? GetIntersection(HashSet<Point> points, Sensor sensor)
    {
        return Orientations.AsParallel().WithDegreeOfParallelism(32)
            .Select(orientation =>
            {
                var transform = orientation.Transform;
                foreach (var pointBase in points)
                {
                    foreach (var pointTest in sensor.Points)
                    {
                        var relative = pointBase - pointTest * transform;

                        var convertedSensorPoints = sensor.Points
                            .Select(p => relative + p * transform).ToList();

                        var intersectionCount = convertedSensorPoints.Count(points.Contains);
                        if (intersectionCount >= OverlapThreshold)
                        {
                            return (relative, orientation, convertedSensorPoints) as (Point relative, Orientation
                                orientation, List<Point> convertedSensorPoints)?;
                        }
                    }
                }

                return null;
            }).FirstOrDefault(s => s is not null);
        return null;
    }
    

    public Int64 CalculatePartTwo()
    {
        var distances =
            from sensor1 in sensors
            from sensor2 in sensors
            where !ReferenceEquals(sensor1, sensor2)
            select sensor1.AbsoluteCoords.Value.ManhattanDistanceFrom(sensor2.AbsoluteCoords.Value);

        return distances.Max();
    }

    private static Sensor[] ParseInput(string input)
    {
        var scannerTitleLine = WS
            .AndR(StringP("--- scanner"))
            .AndR(WS)
            .AndR(Int)
            .AndL(WS)
            .AndL(StringP("---"));
        var coordinateLine = Pipe(
            Int.AndL(StringP(",")),
            Int.AndL(StringP(",")),
            Int,
            (x, y, z) => new Point(x, y, z)
        );
        var coordinateLines = Many1(coordinateLine, sep: Newline.AndTry(NotFollowedByNewline));

        var scannerLine = Pipe(
            scannerTitleLine.AndL(Newline),
            coordinateLines,
            (id, coords) => (id, coords)
        );

        var parser = Many1(scannerLine, sep: Many1(Newline));
        var result = parser.Run(input).GetResult();
        
        return result.Select(r => new Sensor(r.id, r.coords.ToArray())).ToArray();
    }

    private static IEnumerable<Orientation> GetOrientations()
    {
        var indices = new int[] {1, -1};
        var lines = new int[][]
        {
            new[] { 1, 0, 0},
            new[] { 0, 1, 0},
            new[] { 0, 0, 1},
        };

        var source =
            from x in indices
            from y in indices
            from z in indices
            from line1 in lines
            from line2 in lines
            from line3 in lines
            where !ReferenceEquals(line1, line2) && !ReferenceEquals(line1, line3) && !ReferenceEquals(line2, line3)
            select new[] {(x, line1), (y, line2), (z, line3)};

        var sources = source.Select(item =>
        {
            var result = new int[3, 3];
            foreach (var i in Enumerable.Range(0, 3))
            {
                foreach (var j in Enumerable.Range(0, 3))
                {
                    result[i, j] = item[i].Item2[j] * item[i].Item1;
                }
            }

            return result;
        }).ToArray();

        return sources.Select(s => new Orientation(s))
            .DistinctBy(r => Describe(r.Transform))
            .ToArray();
    }

    private static string Describe(int[,] matrix)
    {
        var strings =
            from i in Enumerable.Range(0, 3)
            from j in Enumerable.Range(0, 3)
            select matrix[i, j].ToString();

        return string.Join(",", strings);
    }
}
