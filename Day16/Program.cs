// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;

var input = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToCharArray()).ToArray();

var part1 = GetNumberOfEnergizedTiles(input, new Move(0, -1, 0, 1));

var leftAndRightStartPositions = Enumerable.Range(0, input.Length).SelectMany<int, Move>(r => [new Move(r, -1, 0, 1), new Move(r, input[0].Length, 0, -1)]);
var topAndBottomStartPositions = Enumerable.Range(0, input[0].Length).SelectMany<int, Move>(c => [new Move(-1, c, 1, 0), new Move(input.Length, c, -1, 0)]);

var results = new ConcurrentBag<int>();
var allStartPositions = leftAndRightStartPositions.Concat(topAndBottomStartPositions).ToArray();
Parallel.ForEach(allStartPositions, p => results.Add(GetNumberOfEnergizedTiles(input, p)));

var part2 = results.Max();

Console.WriteLine(part1);
Console.WriteLine(part2);

return;

static int GetNumberOfEnergizedTiles(char[][] input, Move startMove)
{
    var visited = new List<Move>();
    var queue = new Queue<Move>(new []{ startMove });

    while (queue.TryDequeue(out var currentMove))
    {
        var row = currentMove.Y + currentMove.Dy;
        var col = currentMove.X + currentMove.Dx;

        // If the destination falls outside the bounds of the array, skip this move
        if (row < 0 || row >= input.Length || col < 0 || col >= input[0].Length)
            continue;

        var currentChar = input[row][col];

        switch (currentChar)
        {
            case '.':
            case '-' when currentMove.Dx != 0:
            case '|' when currentMove.Dy != 0:
            {
                var newMove = currentMove with { Y = row, X = col };
                if (!visited.Contains(newMove))
                {
                    visited.Add(newMove);
                    queue.Enqueue(newMove);
                }

                break;
            }
            case '/':
            {
                var newMove = new Move(row, col, -currentMove.Dx, -currentMove.Dy);
                if (!visited.Contains(newMove))
                {
                    visited.Add(newMove);
                    queue.Enqueue(newMove);
                }

                break;
            }
            case '\\':
            {
                var newMove = new Move(row, col, currentMove.Dx, currentMove.Dy);
                if (!visited.Contains(newMove))
                {
                    visited.Add(newMove);
                    queue.Enqueue(newMove);
                }

                break;
            }
            case '|':
            {
                Move[] newMoves = [new Move(row, col, 1, 0), new Move(row, col, -1, 0)];
                foreach (var newMove in newMoves)
                {
                    if (!visited.Contains(newMove))
                    {
                        visited.Add(newMove);
                        queue.Enqueue(newMove);
                    }
                }
                break;
            }
            case '-':
            {
                Move[] newMoves = [new Move(row, col, 0, 1), new Move(row, col, 0, -1)];
                foreach (var newMove in newMoves)
                {
                    if (!visited.Contains(newMove))
                    {
                        visited.Add(newMove);
                        queue.Enqueue(newMove);
                    }
                }
                break;
            }
        }
    }

    return visited.Select(x => (x.X, x.Y)).Distinct().ToArray().Length;
}

record Move(int Y, int X, int Dy, int Dx);
