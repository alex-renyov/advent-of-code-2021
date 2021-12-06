namespace AdventOfCode2021.Solutions;

public partial class Day2
{
    private record struct Coordinates(int Depth, int Position);

    private record struct ExtendedCoordinates(int Depth, int Position, int Aim);

    public int CalculatePartOne()
    {
        Console.WriteLine("Day 2. Part 1");

        var values = Input.Split(Environment.NewLine);

        var lastCoordinates = values.Aggregate(new Coordinates(0, 0), (coords, command) =>
        {
            var parts = command.Split(" ");
            var action = parts[0];
            var amount = int.Parse(parts[1]);

            return action switch
            {
                "forward" => coords with { Position = coords.Position + amount },
                "up" => coords with { Depth = coords.Depth - amount },
                "down" => coords with { Depth = coords.Depth + amount },
                _ => throw new Exception($"Invalid command {action}")
            };
        });

        return lastCoordinates.Depth * lastCoordinates.Position;
    }

    public int CalculatePartTwo()
    {
        Console.WriteLine("Day 2. Part 2");

        var values = Input.Split(Environment.NewLine);

        var lastCoordinates = values.Aggregate(new ExtendedCoordinates(0, 0, 0), (coords, command) =>
        {
            var parts = command.Split(" ");
            var action = parts[0];
            var amount = int.Parse(parts[1]);

            return action switch
            {
                "forward" => coords with { Position = coords.Position + amount, Depth = coords.Depth + coords.Aim * amount },
                "up" => coords with { Aim = coords.Aim - amount },
                "down" => coords with { Aim = coords.Aim + amount },
                _ => throw new Exception($"Invalid command {action}")
            };
        });

        return lastCoordinates.Depth * lastCoordinates.Position;
    }
}