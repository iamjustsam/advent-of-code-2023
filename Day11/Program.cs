// See https://aka.ms/new-console-template for more information

var spaceMap = File.ReadLines("input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToCharArray()).ToArray();
var emptySpace = GetEmptySpace(spaceMap);
var galaxies = GetGalaxies(spaceMap).ToArray();
var distances1 = GetDistances(galaxies, emptySpace, 2);
var distances2 = GetDistances(galaxies, emptySpace, 1000000);

var sum1 = distances1.Sum();
var sum2 = distances2.Sum();

Console.WriteLine($"Part 1: {sum1}");
Console.WriteLine($"Part 2: {sum2}");

return;

static EmptySpace GetEmptySpace(char[][] spaceMap)
{
    var emptyRows = spaceMap.Select((row, i) => new { Index = i, IsEmpty = row.All(x => x == '.') }).Where(x => x.IsEmpty).Select(x => x.Index).ToArray();
    var emptyColumns = spaceMap[0].Select((_, i) => new { Index = i, IsEmpty = spaceMap.All(x => x[i] == '.') }).Where(x => x.IsEmpty).Select(x => x.Index).ToArray();

    return new EmptySpace(emptyRows, emptyColumns);
}

static IEnumerable<Galaxy> GetGalaxies(char[][] spaceMap)
{
    for (var y = 0; y < spaceMap.Length; y++)
    {
        for (var x = 0; x < spaceMap[y].Length; x++)
        {
            if (spaceMap[y][x] == '#')
                yield return new Galaxy(x, y);
        }
    }
}

static IEnumerable<long> GetDistances(Galaxy[] galaxies, EmptySpace emptySpace, int expansionRate)
{
    for (var i = 0; i < galaxies.Length; i++)
    {
        for (var j = i + 1; j < galaxies.Length; j++)
        {
            yield return GetDistance(galaxies[i], galaxies[j], emptySpace, expansionRate);
        }
    }
}

static long GetDistance(Galaxy galaxyA, Galaxy galaxyB, EmptySpace emptySpace, int expansionRate)
{
    var minX = Math.Min(galaxyA.X, galaxyB.X);
    var maxX = Math.Max(galaxyA.X, galaxyB.X);
    var minY = Math.Min(galaxyA.Y, galaxyB.Y);
    var maxY = Math.Max(galaxyA.Y, galaxyB.Y);

    var distance = (maxX - minX) + (maxY - minY);

    var emptyRowCount = emptySpace.Rows.Count(x => x >= minY && x <= maxY);
    var emptyColCount = emptySpace.Cols.Count(x => x >= minX && x <= maxX);

    return distance + (emptyRowCount * (expansionRate - 1)) + (emptyColCount * (expansionRate - 1));
}

record EmptySpace(int[] Rows, int[] Cols);
record Galaxy(int X, int Y);
