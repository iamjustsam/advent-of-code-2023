// See https://aka.ms/new-console-template for more information

var input = File.ReadAllText("input.txt");
var patterns = input.Split("\r\n\r\n").Select(x => x.Split("\r\n").Select(y => y.ToCharArray()).Where(y => y.Length != 0).ToArray());
var totalPart1 = 0;
var totalPart2 = 0;

foreach (var pattern in patterns)
{
    var rowPart1 = GetMirrorPart1(pattern);
    var rowPart2 = GetMirrorPart2(pattern);
    totalPart1 += rowPart1 * 100;
    totalPart2 += rowPart2 * 100;

    var colPart1 = GetMirrorPart1(Rotate(pattern));
    var colPart2 = GetMirrorPart2(Rotate(pattern));
    totalPart1 += colPart1;
    totalPart2 += colPart2;
}

Console.WriteLine(totalPart1);
Console.WriteLine(totalPart2);

return;

static int GetMirrorPart2(char[][] pattern)
{
    foreach (var r in Enumerable.Range(1, pattern.Length))
    {
        var above = pattern[..r].Reverse().ToArray();
        var below = pattern[r..];

        var diff = above.Zip(below)
            .SelectMany(x => x.First.Zip(x.Second))
            .Select(x => x.First == x.Second ? 0 : 1)
            .Sum();

        if (diff == 1)
            return r;
    }

    return 0;
}

static int GetMirrorPart1(char[][] pattern)
{
    foreach (var r in Enumerable.Range(1, pattern.Length))
    {
        var above = pattern[..r].Reverse().ToArray();
        var below = pattern[r..];

        above = above[..Math.Min(below.Length, above.Length)];
        below = below[..Math.Min(below.Length, above.Length)];

        if (AreSame(above, below))
            return r;
    }

    return 0;
}

static bool AreSame(char[][] a, char[][] b)
{
    if (a.Length == 0 || b.Length == 0)
        return false;

    if (a.Length != b.Length)
        return false;

    for (var i = 0; i < a.Length; i++)
    {
        if (!a[i].SequenceEqual(b[i]))
            return false;
    }

    return true;
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
