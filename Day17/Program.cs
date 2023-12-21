var input = File.ReadLines("input.txt").Select(x => x.Select(c => c - '0').ToArray()).ToArray();
var part1 = FindShortestPath(input, 1, 3);
var part2 = FindShortestPath(input, 4, 10);

Console.WriteLine(part1);
Console.WriteLine(part2);

return;

static int FindShortestPath(int[][] input, int min, int max)
{
    var startPos = new Point(0, 0);
    var endPos = new Point(input[0].Length - 1, input.Length - 1);
    var initialState = new State(startPos, Vector.Zero, 0);

    var cost = new Dictionary<State, int> { { initialState, 0 } };
    var queue = new PriorityQueue<State, int>([(initialState, 0)]);

    while (queue.TryDequeue(out var currentMove, out var currentMoveCost))
    {
        if (currentMove.Pos == endPos && currentMove.StepCount >= min)
            return currentMoveCost;

        foreach (var move in GetPossibleMoves(currentMove, min, max))
        {
            var moveCost = cost.GetValueOrDefault(move, int.MaxValue);

            if (move.Pos.CheckBounds(input[0].Length, input.Length) && currentMoveCost + input[move.Pos.Y][move.Pos.X] < moveCost)
            {
                cost[move] = currentMoveCost + input[move.Pos.Y][move.Pos.X];
                queue.Enqueue(move, cost[move]);
            }
        }
    }

    return 0;
}

static IEnumerable<State> GetPossibleMoves(State state, int min, int max)
{
    // For the initial move, we can go in all directions
    if (state.Dir == Vector.Zero)
    {
        var left = Vector.Left;
        var right = Vector.Right;
        var up = Vector.Up;
        var down = Vector.Down;

        yield return new State(state.Pos + left, left, 1);
        yield return new State(state.Pos + right, right, 1);
        yield return new State(state.Pos + up, up, 1);
        yield return new State(state.Pos + down, down, 1);

        yield break;
    }

    // We can only go left or right if we are beyond the minimum number of steps
    if (state.StepCount >= min)
    {
        var left = state.Dir.Rotate(-90);
        var right = state.Dir.Rotate(90);

        yield return new State(state.Pos + left, left, 1);
        yield return new State(state.Pos + right, right, 1);
    }

    // We can only go straight if we are under the maximum number of steps
    if (state.StepCount < max)
    {
        yield return new State(state.Pos + state.Dir, state.Dir, state.StepCount + 1);
    }
}

internal record State(Point Pos, Vector Dir, int StepCount);

internal record Point(int X, int Y)
{
    public bool CheckBounds(int maxX, int maxY)
    {
        return X >= 0 && X < maxX && Y >= 0 && Y < maxY;
    }

    public static Point operator +(Point point, Vector vector)
    {
        return new Point(point.X + vector.X, point.Y + vector.Y);
    }
}

internal record Vector(int X, int Y)
{
    public static Vector Zero => new Vector(0, 0);
    public static Vector Up => new Vector(0, -1);
    public static Vector Right => new Vector(1, 0);
    public static Vector Down => new Vector(0, 1);
    public static Vector Left => new Vector(-1, 0);

    public Vector Rotate(int deg)
    {
        var rad = deg * (Math.PI / 180);
        var newX = X * Math.Cos(rad) - Y * Math.Sin(rad);
        var newY = X * Math.Sin(rad) + Y * Math.Cos(rad);

        return new Vector((int)newX, (int)newY);
    }
}
