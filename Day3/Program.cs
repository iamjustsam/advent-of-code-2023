// See https://aka.ms/new-console-template for more information

var file = File.ReadAllLines("input.txt");

var flags = new bool[140, 140];

var numbersToSum = new List<int>();
var gearRatios = new List<int>();

for (var y = 0; y < 140; y++)
{
    for (var x = 0; x < 140; x++)
    {
        var c = file[y][x];
        var isNumber = char.IsNumber(c);
        var isLetter = char.IsLetter(c);
        var isSymbol = !isNumber && !isLetter && c != '.';

        if (isSymbol)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var result = FindSurroundingNumbers(x, y, file);
            numbersToSum.AddRange(result.Numbers);
            gearRatios.Add(result.GearRatio);
        }

        if (flags[y, x])
            Console.ForegroundColor = ConsoleColor.Green;

        Console.Write(file[y][x]);

        Console.ForegroundColor = ConsoleColor.White;
    }
    Console.Write('\n');
}

Console.WriteLine($"Total: {numbersToSum.Sum()}");
Console.WriteLine($"Gear ratio: {gearRatios.Sum()}");

SearchResult FindSurroundingNumbers(int x, int y, string[] data)
{
    var isGear = data[y][x] == '*';
    var numbers = new[]
    {
        FindNumber(x, y, Direction.TopLeft, data),
        FindNumber(x, y, Direction.TopMid, data),
        FindNumber(x, y, Direction.TopRight, data),
        FindNumber(x, y, Direction.Left, data),
        FindNumber(x, y, Direction.Right, data),
        FindNumber(x, y, Direction.BottomLeft, data),
        FindNumber(x, y, Direction.BottomMid, data),
        FindNumber(x, y, Direction.BottomRight, data),
    };

    var gearPartNumbers = numbers.Where(n => n > 0).ToArray();

    var gearRatio = isGear && gearPartNumbers.Length == 2 ? gearPartNumbers.Aggregate(1, (total, number) => total * number) : 0;

    return new SearchResult(numbers, gearRatio);
}

int FindNumber(int x, int y, Direction direction, string[] data)
{
    if (!CheckBounds(x, y, direction))
        return 0;

    var yPos = direction switch
    {
        Direction.TopLeft or Direction.TopMid or Direction.TopRight => y - 1,
        Direction.Left or Direction.Right => y,
        Direction.BottomLeft or Direction.BottomMid or Direction.BottomRight => y + 1,
        _ => throw new Exception("Invalid direction")
    };

    var result = FindNumberInLine(x, yPos, direction, data[yPos]);

    return result;
}

int FindNumberInLine(int x, int y, Direction direction, string data)
{
    var n = 0;

    if (direction is Direction.TopLeft or Direction.Left or Direction.BottomLeft)
    {
        if (!char.IsNumber(data[x - 1]))
            return 0;

        n = GetNumberAtPos(x - 1, y, data);
    }

    if (direction is Direction.TopRight or Direction.Right or Direction.BottomRight)
    {
        if (!char.IsNumber(data[x + 1]))
            return 0;

        n = GetNumberAtPos(x + 1, y, data);
    }

    if (direction is Direction.TopMid or Direction.BottomMid)
    {
        if (!char.IsNumber(data[x]))
            return 0;

        n = GetNumberAtPos(x, y, data);
    }

    return n;
}

int GetNumberAtPos(int x, int y, string data)
{
    var startPos = x;
    while (startPos >= 0 && char.IsNumber(data[startPos]))
        startPos--;

    var endPos = startPos + 1;
    while (endPos <= 139 && char.IsNumber(data[endPos]))
    {
        if (flags[y, endPos])
            return 0;

        flags[y, endPos] = true;
        endPos++;
    }

    var number = data[(startPos + 1)..endPos];

    return int.Parse(number);
}

static bool CheckBounds(int x, int y, Direction direction)
{
    return direction switch
    {
        Direction.TopLeft when x == 0 || y == 0 => false,
        Direction.TopMid when y == 0 => false,
        Direction.TopRight when x == 139 || y == 0 => false,
        Direction.Left when x == 0 => false,
        Direction.Right when x == 139 => false,
        Direction.BottomLeft when x == 0 || y == 139 => false,
        Direction.BottomMid when y == 139 => false,
        Direction.BottomRight when x == 139 || y == 139 => false,
        _ => true
    };
}

enum Direction
{
    TopLeft,
    TopMid,
    TopRight,
    Left,
    Right,
    BottomLeft,
    BottomMid,
    BottomRight
}

record SearchResult(int[] Numbers, int GearRatio);
