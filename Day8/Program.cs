// See https://aka.ms/new-console-template for more information

var input = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
var directions = input[0].Select(x => x == 'L' ? 0 : 1).ToArray();
var dictionary = input[1..].Select(x => x.Split(" = ")).ToDictionary(x => x[0], x => x[1].Trim('(', ')').Split(", "));

var part1StartNode = "AAA";
var part1Length = GetPathLength(part1StartNode, directions, dictionary, x => x != "ZZZ");

var part2StartNodes = dictionary.Keys.Where(x => x.EndsWith('A'));
var part2Lengths = part2StartNodes.Select(x => GetPathLength(x, directions, dictionary, y => !y.EndsWith('Z')));

var part2Length = part2Lengths.Aggregate(1L, LeastCommonMultiple);

Console.WriteLine(part1Length);
Console.WriteLine(part2Length);

return;

int GetPathLength(string startKey, int[] directions, IDictionary<string, string[]> dictionary, Func<string, bool> condition)
{
    var numberOfSteps = 0;
    var curr = startKey;
    var enumerator = directions.GetEnumerator();

    while (condition(curr))
    {
        var hasNext = enumerator.MoveNext();
        if (!hasNext)
        {
            enumerator.Reset();
            enumerator.MoveNext();
        }

        var nextDirection = (int)enumerator.Current;
        curr = dictionary[curr][nextDirection];
        numberOfSteps++;
    }

    return numberOfSteps;
}

static long GreatestCommonFactor(long a, long b)
{
    while (b != 0)
    {
        var tmp = b;
        b = a % b;
        a = tmp;
    }

    return a;
}

static long LeastCommonMultiple(long a, int b)
{
    return a / GreatestCommonFactor(a, b) * b;
}
