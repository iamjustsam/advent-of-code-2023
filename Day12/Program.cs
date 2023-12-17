// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;

var cache = new Dictionary<string, long>();

var input = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(ParseLine).ToArray();

var part1 = input.Select(x => GetPossibleArrangements(x.Data, x.Groups)).Sum();
var part2 = input.Select(x =>
{
    var repeatedData = string.Join('?', Enumerable.Repeat(x.Data, 5));
    var repeatedGroups = Enumerable.Repeat(x.Groups, 5).SelectMany(y => y);

    return GetPossibleArrangements(repeatedData, repeatedGroups.ToArray());
}).ToArray();

var sw = Stopwatch.StartNew();
Console.WriteLine(part1);
Console.WriteLine(part2.Sum());
sw.Stop();
Console.WriteLine(sw.Elapsed);
Console.WriteLine(cache.Count);

return;

static SpringData ParseLine(string line)
{
    var parts = line.Split(' ');
    var groups = parts[1].Split(',').Select(int.Parse).ToArray();

    return new(Simplify(parts[0]), groups);
}

long GetPossibleArrangements(ReadOnlySpan<char> sequence, int[] groups)
{
    if (sequence.IsEmpty)
        return groups.Length == 0 ? 1 : 0;

    if (groups.Length == 0)
        return sequence.Contains('#') ? 0 : 1;

    var cacheKey = $"${sequence}{string.Join(',', groups)}";

    if (cache.TryGetValue(cacheKey, out var value))
        return value;

    var result = 0L;

    if (sequence[0] is '.' or '?')
        result += GetPossibleArrangements(sequence[1..], groups);

    if(sequence[0] is '#' or '?')
        if (groups[0] <= sequence.Length && !sequence[..groups[0]].Contains('.') && (groups[0] == sequence.Length || sequence[groups[0]] != '#'))
            result += GetPossibleArrangements(sequence[Math.Min(groups[0] + 1, sequence.Length)..], groups[1..]);

    cache[cacheKey] = result;

    return result;
}

static string Simplify(string input)
{
    if (input.Count(x => x == '.') <= 1)
        return input;

    var sb = new StringBuilder();
    var prev = '\0';
    foreach (var c in input)
    {
        var skip = c == '.' && prev == '.';
        prev = c;
        if(skip)
            continue;

        sb.Append(c);
    }

    return sb.ToString();
}

internal record SpringData(string Data, int[] Groups);
