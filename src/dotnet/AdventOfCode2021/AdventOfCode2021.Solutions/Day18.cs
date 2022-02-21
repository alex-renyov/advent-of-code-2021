using System.Text;

namespace AdventOfCode2021.Solutions;

public partial class Day18 : IDay
{
    private enum TokenType
    {
        Unknown = 0,
        OpenBrace,
        CloseBrace,
        Comma,
        Value
    }
    private interface IToken
    {
        TokenType Type { get; }
    }

    private class OpenBraceToken : IToken
    {
        public TokenType Type => TokenType.OpenBrace;
        public override String ToString() => "[";
    }
    
    private class CloseBraceToken : IToken
    {
        public TokenType Type => TokenType.CloseBrace;
        public override String ToString() => "]";
    }
    
    private class CommaToken : IToken
    {
        public TokenType Type => TokenType.Comma;
        public override String ToString() => ",";
    }
    
    private class ValueToken : IToken
    {
        public Int32 Value { get; }
        
        public TokenType Type => TokenType.Value;

        public ValueToken(int value)
        {
            Value = value;
        }
        
        public override String ToString() => Value.ToString();
    }
    
    public long CalculatePartOne()
    {
        var input = ParseInput();
        var result = input.Aggregate((left, right) =>
        {
            var sum = Sum(left, right);
            return Reduce(sum);
        });
        return Magnitude(result);
    }

    public long CalculatePartTwo()
    {
        var input = ParseInput();

        var max = 0L;

        foreach (var x in input)
        {
            foreach (var y in input)
            {
                if (ReferenceEquals(x, y))
                {
                    continue;
                }

                var sum = Sum(x, y);
                var reduced = Reduce(sum);
                var magnitude = Magnitude(reduced);
                
                max = Math.Max(max, magnitude);
            }
        }

        return max;
    }

    private static string Print(IToken[] tokens)
    {
        var sb = new StringBuilder();
        foreach (var token in tokens)
        {
            sb = token switch
            {
                OpenBraceToken => sb.Append('['),
                CloseBraceToken => sb.Append(']'),
                CommaToken => sb.Append(','),
                ValueToken valueToken => sb.Append(valueToken.Value),
                _ => sb
            };
        }

        return sb.ToString();
    }

    private static long Magnitude(IToken[] source)
    {
        if (source.Length == 1 && source[0] is ValueToken valueToken)
        {
            return valueToken.Value;
        }

        var trimmed = source.Skip(1).Take(source.Length - 2).ToArray();
        
        var opened = 0;
        foreach (var item in trimmed.Select((token, index) => new { token, index }))
        {
            if (item.token.Type == TokenType.OpenBrace)
            {
                opened++;
            }
            else if (item.token.Type == TokenType.CloseBrace)
            {
                opened--;
            }
            else if (item.token.Type == TokenType.Comma)
            {
                if (opened != 0)
                {
                    continue;
                }

                var left = trimmed.Take(item.index).ToArray();
                var right = trimmed.Skip(item.index + 1).ToArray();
                return Magnitude(left) * 3 + Magnitude(right) * 2;
            }
        }

        throw new InvalidOperationException("Invalid sequence");
    }

    private static IToken[] Sum(IToken[] left, IToken[] right)
    {
        var result = new List<IToken>();
        result.Add(new OpenBraceToken());
        result.AddRange(left);
        result.Add(new CommaToken());
        result.AddRange(right);
        result.Add(new CloseBraceToken());
        return result.ToArray();
    }

    private static IToken[] Reduce(IToken[] source)
    {
        var result = source;
        while (true)
        {
            var exploded = Explode(result);
            if (exploded != null)
            {
                result = exploded;
                continue;
            }

            var splitted = Split(result);
            if (splitted != null)
            {
                result = splitted;
                continue;
            }

            break;
        }

        return result;
    }

    private static IToken[]? Explode(IToken[] source)
    {
        ReadOnlySpan<IToken> full = source;
        ReadOnlySpan<IToken> left = ReadOnlySpan<IToken>.Empty;
        ReadOnlySpan<IToken> pair = ReadOnlySpan<IToken>.Empty;
        ReadOnlySpan<IToken> right = ReadOnlySpan<IToken>.Empty;

        foreach (var i in Enumerable.Range(1, source.Length - 3))
        {
            var slice = full.Slice(i, 3);
            var leftOfSlice = full.Slice(0, i - 1);

            if ((slice[0].Type, slice[1].Type, slice[2].Type) is not (TokenType.Value, TokenType.Comma, TokenType.Value))
            {
                continue;
            }

            var opened = leftOfSlice.ToArray().Select(item =>
            {
                return item.Type switch
                {
                    TokenType.OpenBrace => 1,
                    TokenType.CloseBrace => -1,
                    _ => 0
                };
            }).Sum();

            if (opened == 4)
            {
                left = leftOfSlice;
                pair = slice;
                right = full.Slice(i + 4);
                break;
            }
        }

        if (pair.IsEmpty)
        {
            return null;
        }

        var leftValue = (pair[0] as ValueToken).Value;
        var rightValue = (pair[2] as ValueToken).Value;

        var leftArray = left.ToArray();
        var rightArray = right.ToArray();
        
        var leftValueNode = leftArray.Reverse().OfType<ValueToken>().FirstOrDefault();
        if (leftValueNode != null)
        {
            leftArray = leftArray.Select(item =>
                    ReferenceEquals(item, leftValueNode) ? new ValueToken(leftValueNode.Value + leftValue) : item)
                .ToArray();
        }

        var rightValueNode = rightArray.OfType<ValueToken>().FirstOrDefault();
        if (rightValueNode != null)
        {
            rightArray = rightArray.Select(item =>
                    ReferenceEquals(item, rightValueNode) ? new ValueToken(rightValueNode.Value + rightValue) : item)
                .ToArray();
        }

        return leftArray.Concat(new IToken[] { new ValueToken(0)}).Concat(rightArray).ToArray();
    }

    private static IToken[]? Split(IToken[] source)
    {
        ReadOnlySpan<IToken> full = source;
        ReadOnlySpan<IToken> left = ReadOnlySpan<IToken>.Empty;
        ValueToken number = null;
        ReadOnlySpan<IToken> right = ReadOnlySpan<IToken>.Empty;

        foreach (var item in source.Select((token, index) => new { token, index }))
        {
            if (item is { token: ValueToken { Value: >= 10 } })
            {
                left = full.Slice(0, item.index);
                number = item.token as ValueToken;
                right = full.Slice(item.index + 1);
                break;
            }
        }

        if (number == null)
        {
            return null;
        }

        decimal value = number.Value;
        var leftValue = (int)Math.Round(value / 2, MidpointRounding.ToZero);
        var rightValue = (int)Math.Round(value / 2, MidpointRounding.ToPositiveInfinity);

        var newPair = new IToken[]
        {
            new OpenBraceToken(),
            new ValueToken(leftValue),
            new CommaToken(),
            new ValueToken(rightValue),
            new CloseBraceToken()
        };

        return left.ToArray().Concat(newPair).Concat(right.ToArray()).ToArray();
    }
    
    private static IEnumerable<IToken> ParseLine(string line)
    {
        var digits = new List<char>();
        foreach (var ch in line.ToCharArray())
        {
            if (char.IsDigit(ch))
            {
                digits.Add(ch);
            }
            else
            {
                if (digits.Any())
                {
                    yield return new ValueToken(int.Parse(new string(digits.ToArray())));
                    digits.Clear();
                }
                if (ch == '[')
                {
                    yield return new OpenBraceToken();
                }
                else if (ch == ']')
                {
                    yield return new CloseBraceToken();
                }
                else if (ch == ',')
                {
                    yield return new CommaToken();
                }
            }
            
        }
    }
    
    private static IToken[][] ParseInput()
    {
        return Input.Split(Environment.NewLine).Select(s => ParseLine(s).ToArray()).ToArray();
    }
}
