// See https://aka.ms/new-console-template for more information

var input = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToCharArray()).ToArray();
var maxLoad = input.Length;

var loadPart1 = Rotate(Rotate(input)
    .Select(x => string.Join('#', new string(x).Split('#').Select(y => new string(y.OrderByDescending(e => e).ToArray()))).ToCharArray())
    .ToArray())
    .Select((x, i) => x.Count(y => y == 'O') * (maxLoad - i))
    .Sum();

// To find the 1 billionth iteration, let's first check if we can find a loop by cycling until we find a recurring pattern
var visited = new Dictionary<string, int>();
var savedPatterns = new List<char[][]>();
var iteration = 0;
var result = input;
string key;

while (true)
{
    iteration++;
    result = Cycle(result);
    key = string.Join(null, result.Select(x => new string(x)));
    if (visited.ContainsKey(key))
        break;

    visited[key] = iteration;
    savedPatterns.Add(result);
}

var firstIndex = visited[key];
var resultIndex = (1000000000 - firstIndex) % (iteration - firstIndex) + firstIndex;
result = savedPatterns[resultIndex - 1];

var loadPart2 = result.Select((x, i) => x.Count(y => y == 'O') * (maxLoad - i)).Sum();

Console.WriteLine(loadPart1);
Console.WriteLine(loadPart2);

return;

static char[][] Cycle(char[][] input)
{
    var result = Enumerable.Range(0, 4).Aggregate(input, (current, _) => Tilt(Rotate(current)).Select(x => x.Reverse().ToArray()).ToArray());

    return result;
}

static char[][] Tilt(char[][] input)
{
    return input
        .Select(x => string.Join('#', new string(x).Split('#').Select(y => new string(y.OrderByDescending(e => e).ToArray()))).ToCharArray())
        .ToArray();
}

static char[][] Rotate(char[][] input)
{
    var length = input[0].Length;
    var result = new char[length][];

    for (var i = 0; i < length; i++)
    {
        result[i] = input.Select(x => x[i]).ToArray();
    }

    return result;
}
