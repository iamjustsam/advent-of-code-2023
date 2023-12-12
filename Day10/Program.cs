// See https://aka.ms/new-console-template for more information

var input = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToCharArray()).ToArray();

Console.WriteLine($"Part 1: {SolvePart1(input)}");
Console.WriteLine($"Part 2: {SolvePart2(input)}");

return;

static int SolvePart1(char[][] maze)
{
    var stepCount = 1;
    var startPos = GetStartingPoint(maze);
    var currentPositions = GetStartingPositions(maze, startPos).ToArray();
    if (currentPositions.Length != 2)
        throw new Exception("Invalid starting positions");

    while (!currentPositions[0].Point.Equals(currentPositions[1].Point))
    {
        try
        {
            currentPositions = currentPositions.Select(p => GetNextPosition(maze, p)).ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Step: {stepCount}");
            Console.WriteLine(e);
            throw;
        }
        stepCount++;
    }

    return stepCount;
}

static int SolvePart2(char[][] maze)
{
    var startPos = GetStartingPoint(maze);
    var vertices = new List<Point> { startPos };

    var boundayPointsCount = 1;
    var currentPosition = GetStartingPositions(maze, startPos).ToArray()[0];

    while (!currentPosition.Point.Equals(startPos))
    {
        var currentChar = maze[currentPosition.Point.Y][currentPosition.Point.X];
        if(currentChar is 'F' or '7' or 'L' or 'J')
            vertices.Add(currentPosition.Point);

        currentPosition = GetNextPosition(maze, currentPosition);
        boundayPointsCount++;
    }

    var area = GetArea(vertices);

    return area - boundayPointsCount / 2 + 1;
}

static Point North(Point p) => p with { Y = p.Y - 1 };
static Point East(Point p) => p with { X = p.X + 1 };
static Point South(Point p) => p with { Y = p.Y + 1 };
static Point West(Point p) => p with { X = p.X - 1 };

static Position GetNextPosition(char[][] maze, Position p)
{
    var north = North(p.Point);
    var east = East(p.Point);
    var south = South(p.Point);
    var west = West(p.Point);
    var currentChar = maze[p.Point.Y][p.Point.X];

    return currentChar switch
    {
        '|' when p.Direction is Direction.North => new(north, Direction.North),
        '|' when p.Direction is Direction.South => new(south, Direction.South),
        '-' when p.Direction is Direction.East => new(east, Direction.East),
        '-' when p.Direction is Direction.West => new(west, Direction.West),
        'L' when p.Direction is Direction.South => new(east, Direction.East),
        'L' when p.Direction is Direction.West => new(north, Direction.North),
        'J' when p.Direction is Direction.South => new(west, Direction.West),
        'J' when p.Direction is Direction.East => new(north, Direction.North),
        '7' when p.Direction is Direction.East => new(south, Direction.South),
        '7' when p.Direction is Direction.North => new(west, Direction.West),
        'F' when p.Direction is Direction.West => new(south, Direction.South),
        'F' when p.Direction is Direction.North => new(east, Direction.East),
        _ => throw new Exception("Invalid position")
    };
}

static Point GetStartingPoint(char[][] maze)
{
    for (var y = 0; y < maze.Length; y++)
    {
        for (var x = 0; x < maze[y].Length; x++)
        {
            if (maze[y][x] == 'S')
                return new(x, y);
        }
    }

    throw new Exception("No starting position found");
}

static IEnumerable<Position> GetStartingPositions(char[][] maze, Point p)
{
    var north = North(p);
    var east = East(p);
    var south = South(p);
    var west = West(p);

    if (GetValueAtPoint(maze, north) is '|' or '7' or 'F')
        yield return new(north, Direction.North);

    if (GetValueAtPoint(maze, east) is '-' or 'J' or '7')
        yield return new(east, Direction.East);

    if (GetValueAtPoint(maze, south) is '|' or 'L' or 'J')
        yield return new(south, Direction.South);

    if (GetValueAtPoint(maze, west) is '-' or 'L' or 'F')
        yield return new(west, Direction.West);
}

static char GetValueAtPoint(char[][] maze, Point p)
{
    var x = Math.Min(maze[0].Length, Math.Max(0, p.X));
    var y = Math.Min(maze.Length, Math.Max(0, p.Y));

    return maze[y][x];
}

static int GetArea(List<Point> vertices)
{
    var area = 0;

    for (int i = 0; i < vertices.Count; i++)
    {
        var nextIndex = (i + 1) % vertices.Count; // Will return 0 when we reach the end of the array so we can access the first element to close the loop
        var currentPoint = vertices[i];
        var nextPoint = vertices[nextIndex];

        area += currentPoint.X * nextPoint.Y - currentPoint.Y * nextPoint.X;
    }

    return Math.Abs(area) / 2;
}

internal record Point(int X, int Y);
internal record Position(Point Point, Direction Direction);

internal enum Direction
{
    North,
    East,
    South,
    West
}
