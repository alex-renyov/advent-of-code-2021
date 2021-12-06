namespace AdventOfCode2021.Solutions;

public partial class Day4
{
    public int CalculatePartOne()
    {
        Console.WriteLine("Day 4. Part 1");

        var numbers = GetInput();
        var (size, boards) = GetBoards();

        var drawn = new HashSet<int>();

        foreach (var number in numbers)
        {
            drawn.Add(number);

            var winning = boards.FirstOrDefault(b => IsWinning(b, size, drawn));

            if (winning == null)
            {
                continue;
            }

            var unmarkedSum = GetValues(winning, size).Where(num => !drawn.Contains(num)).Sum();
            return unmarkedSum * number;
        }

        return 0;
    }

    public int CalculatePartTwo()
    {
        Console.WriteLine("Day 4. Part 2");

        var numbers = GetInput();
        var (size, boards) = GetBoards();

        var fullDrawn = numbers.ToHashSet();
        var winningBoards = boards.Where(b => IsWinning(b, size, fullDrawn)).ToArray();

        var drawn = new HashSet<int>();
        var remainingBoards = winningBoards;

        foreach (var number in numbers)
        {
            drawn.Add(number);

            var winning = remainingBoards.Where(b => IsWinning(b, size, drawn)).ToArray();
            if (winning.Length == 0)
            {
                continue;
            }

            remainingBoards = remainingBoards.Except(winning).ToArray();
            if (remainingBoards.Length == 0)
            {
                var unmarkedSum = GetValues(winning[0], size).Where(num => !drawn.Contains(num)).Sum();
                return unmarkedSum * number;
            }
        }

        return 0;
    }

    private static bool IsWinning(int[,] board, int size, IReadOnlySet<int> drawn)
    {
        return MakeLines(board, size).Any(line => line.All(drawn.Contains));
    }

    private static IEnumerable<int[]> MakeLines(int[,] board, int size)
    {
        for (var i = 0; i < size; i++)
        {
            yield return Enumerable.Range(0, size).Select(j => board[i, j]).ToArray();
            yield return Enumerable.Range(0, size).Select(j => board[j, i]).ToArray();
        }
    }

    private static IEnumerable<int> GetValues(int[,] board, int size)
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                yield return board[i, j];
            }
        }
    }

    private static int[] GetInput()
    {
        return Numbers.Split(',').Select(int.Parse).ToArray();
    }

    private static (int size, int[][,] boards) GetBoards()
    {
        var boards = Boards.Split(Environment.NewLine + Environment.NewLine)
          .Select(b =>
          {
              var result = new int[5, 5];
              var lines = b.Trim().Split(Environment.NewLine);
              for (var i = 0; i < 5; i++)
              {
                  var lineParts = lines[i].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                  for (var j = 0; j < 5; j++)
                  {
                      result[i, j] = lineParts[j];
                  }
              }

              return result;
          })
          .ToArray();

        var size = boards[0].GetLength(0);

        return (size, boards);
    }
}