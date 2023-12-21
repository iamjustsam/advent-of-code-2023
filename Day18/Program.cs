var inputPart1 = File.ReadLines("input.txt").Select(ParsePart1).ToArray();
var part1 = Measure(inputPart1);

var inputPart2 = File.ReadLines("input.txt").Select(ParsePart2).ToArray();
var part2 = Measure(inputPart2);

Console.WriteLine(part1);
Console.WriteLine(part2);

return;

static long Measure(IEnumerable<Instruction> instructions)
{
    var vertex = new Point(0, 0);
    var vertices = new List<Point>();
    var trenchLength = 0L;

    foreach (var instruction in instructions)
    {
        vertex = instruction.Direction switch
        {
            'U' => vertex with { Y = vertex.Y - instruction.Length },
            'D' => vertex with { Y = vertex.Y + instruction.Length },
            'L' => vertex with { X = vertex.X - instruction.Length },
            'R' => vertex with { X = vertex.X + instruction.Length },
            _ => throw new Exception("Invalid direction")
        };

        vertices.Add(vertex);
        trenchLength += instruction.Length;
    }

    var area = GetArea(vertices);
    var insideArea = area - trenchLength / 2 + 1;

    return insideArea + trenchLength;
}

static Instruction ParsePart1(string input)
{
    var parts = input.Split(' ');

    return new Instruction(parts[0][0],  int.Parse(parts[1]));
}

static Instruction ParsePart2(string input)
{
    var parts = input.Split(' ');
    var instructionPart = parts[2].Trim('(', ')');

    var distance = instructionPart[1..^1];
    var direction = instructionPart[^1] switch
    {
        '0' => 'R',
        '1' => 'D',
        '2' => 'L',
        '3' => 'U',
        _ => throw new Exception("Invalid direction")
    };

    return new Instruction(direction, Convert.ToInt32(distance, fromBase: 16));
}

static long GetArea(IReadOnlyList<Point> vertices)
{
    var area = 0L;

    for (var i = 0; i < vertices.Count; i++)
    {
        var nextIndex = (i + 1) % vertices.Count; // Will return 0 when we reach the end of the array so we can access the first element to close the loop
        var currentPoint = vertices[i];
        var nextPoint = vertices[nextIndex];

        area += currentPoint.X * nextPoint.Y - currentPoint.Y * nextPoint.X;
    }

    return Math.Abs(area) / 2;
}

record Instruction(char Direction, long Length);
record Point(long X, long Y);
