namespace AdventOfCode2021.Solutions;

public sealed partial class Day16 : IDay
{
    private abstract record Packet(int Version, int TypeId);

    private record LiteralPacket(int Version, int TypeId, long Value) : Packet(Version, TypeId);
    
    private record OperatorPacket(int Version, int TypeId, Packet[] Children) : Packet(Version, TypeId);
    
    public long CalculatePartOne()
    {
        var input = ParseInput();
        var (parsed, _) = ParsePacket(input);

        static int VersionsSum(Packet packet)
        {
            return packet switch
            {
                LiteralPacket p => p.Version,
                OperatorPacket p => p.Version + p.Children.Sum(VersionsSum),
                _ => throw new InvalidOperationException("Unsupported packet type")
            };
        }

        return VersionsSum(parsed);
    }

    public long CalculatePartTwo()
    {
        var input = ParseInput();
        var (parsed, _) = ParsePacket(input);

        static long PacketValue(Packet packet)
        {
            return packet switch
            {
                LiteralPacket p => p.Value,
                OperatorPacket { TypeId: 0 } p => p.Children.Sum(PacketValue),
                OperatorPacket { TypeId: 1 } p => p.Children.Aggregate(1L, (acc, p) => acc * PacketValue(p)),
                OperatorPacket { TypeId: 2 } p => p.Children.Min(PacketValue),
                OperatorPacket { TypeId: 3 } p => p.Children.Max(PacketValue),
                OperatorPacket { TypeId: 5 } p => PacketValue(p.Children[0]) > PacketValue(p.Children[1]) ? 1L : 0L,
                OperatorPacket { TypeId: 6 } p => PacketValue(p.Children[0]) < PacketValue(p.Children[1]) ? 1L : 0L,
                OperatorPacket { TypeId: 7 } p => PacketValue(p.Children[0]) == PacketValue(p.Children[1]) ? 1L : 0L,
                _ => throw new InvalidOperationException("Invalid packet")
            };
        }

        return PacketValue(parsed);
    }

    private static (Packet, Memory<byte>) ParsePacket(Memory<byte> input)
    {
        var (version, remaining) = ReadBits(input, 3);
        (var typeId, remaining) = ReadBits(remaining, 3);

        if (typeId == 4)
        {
            var data = new List<byte>();
            while (true)
            {
                var flag = remaining[0..1].ToArray()[0];
                var fragment = remaining[1..5];
                remaining = remaining[5..^0];
                data.AddRange(fragment.ToArray());
                if (flag == 0)
                {
                    break;
                }
            }
            
            return (new LiteralPacket(version, typeId, GetLongValue(data.ToArray())), remaining);
        }

        (var lengthTypeId, remaining) = ReadBits(remaining, 1);
        if (lengthTypeId == 0)
        {
            (var totalLength, remaining) = ReadBits(remaining, 15);
            var targetLength = remaining.Length - totalLength;
            var children = new List<Packet>();
            while (remaining.Length > targetLength)
            {
                (var newPacket, remaining) = ParsePacket(remaining);
                children.Add(newPacket);
            }
            return (new OperatorPacket(version, typeId, children.ToArray()), remaining);
        }
        else
        {
            (var totalNumber, remaining) = ReadBits(remaining, 11);
            var children = new List<Packet>(totalNumber);
            
            foreach (var i in Enumerable.Range(0, totalNumber))
            {
                (var newPacket, remaining) = ParsePacket(remaining);
                children.Add(newPacket);
            }

            return (new OperatorPacket(version, typeId, children.ToArray()), remaining);
        }
    }

    private static (int value, Memory<byte> remaining) ReadBits(Memory<byte> bits, int count)
    {
        return (GetValue(bits[0..count]), bits[count..^0]);
    }

    private static int GetValue(Memory<byte> memory)
    {
        return Convert.ToInt32(new string(memory.ToArray().Select(b => b.ToString()[0]).ToArray()), 2);
    }
    
    private static long GetLongValue(Memory<byte> memory)
    {
        return Convert.ToInt64(new string(memory.ToArray().Select(b => b.ToString()[0]).ToArray()), 2);
    }

    private static Memory<byte> ParseInput()
    {
        return Input.Select(ch => Convert.ToByte(ch.ToString(), 16))
            .SelectMany(b => Convert.ToString(b, 2).PadLeft(4, '0').ToCharArray().Select(ch => byte.Parse(ch.ToString())))
            .ToArray();
    }
}
