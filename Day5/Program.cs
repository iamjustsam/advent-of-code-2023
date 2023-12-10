var input = File.ReadAllText("input.txt");
var chunks = input.Split("\r\n\r\n");
var seeds = chunks[0].Split(' ')[1..].Select(long.Parse).ToArray();
var categories = chunks[1..].Select(x => x.Split("\r\n").Skip(1).Select(y => y.Split(' ').Select(long.Parse).ToArray()).ToArray()).ToArray();
var location = seeds.Aggregate(long.MaxValue, (loc, seed) => Math.Min(loc, categories.Aggregate(seed, Map)));
var seedRanges = GetSeedRanges(seeds).ToArray();
var mappedRanges = seedRanges.SelectMany(range => categories.Aggregate(range, MapRanges));
var locationp2 = mappedRanges.Aggregate(long.MaxValue, (loc, range) => Math.Min(loc, range.Start));

Console.WriteLine(location);
Console.WriteLine(locationp2);

static long Map(long input, long[][] category)
{
    var map = category.SingleOrDefault(x => x[1] <= input && x[1] - 1 + x[2] >= input);
    if (map is null)
        return input;

    var diff = input - map[1];
    return map[0] + diff;
}

static IEnumerable<List<SeedRange>> GetSeedRanges(IEnumerable<long> seedInput)
{
    using var enumerator = seedInput.GetEnumerator();
    while (enumerator.MoveNext())
    {
        var start = enumerator.Current;
        enumerator.MoveNext();
        var end = start - 1 + enumerator.Current;

        yield return new List<SeedRange> { new(start, end) };
    }
}

static long? SearchNumberInAlmanacMap(long[] almanacMap, long number)
{
    var sourceStart = almanacMap[1];
    var destinationStart = almanacMap[0];
    var rangeLength = almanacMap[2];

    var diff = number - sourceStart;

    return 0 <= diff && diff < rangeLength ? destinationStart + diff : null;
}

static List<SeedRange?> SearchRangeInAlmanacMap(long[] almanacMap, SeedRange range)
{
    var sourceStart = almanacMap[1];
    var rangeLength = almanacMap[2];

    var result = new List<SeedRange?>();
    var mapRangeStart = sourceStart;
    var mapRangeEnd = sourceStart + rangeLength - 1;

    if(range.Start < mapRangeStart)
        result.Add(range with { End = Math.Min(range.End, mapRangeStart - 1) });
    else
        result.Add(null);

    if (mapRangeStart <= range.End && range.Start <= mapRangeEnd)
        result.Add(new SeedRange(SearchNumberInAlmanacMap(almanacMap, Math.Max(mapRangeStart, range.Start))!.Value, SearchNumberInAlmanacMap(almanacMap, Math.Min(mapRangeEnd, range.End))!.Value));
    else result.Add(null);

    if(mapRangeEnd < range.End)
        result.Add(range with { Start = Math.Max(mapRangeEnd + 1, range.Start)});
    else result.Add(null);

    return result;
}

static List<SeedRange> MapRanges(List<SeedRange> ranges, long[][] category) => ranges.SelectMany(x => MapRange(x, category)).ToList();

static List<SeedRange> MapRange(SeedRange range, long[][] category)
{
    var result = new List<SeedRange>();
    var toSearch = new List<SeedRange> { range };
    foreach (var map in category)
    {
        var unmatched = new List<SeedRange>();
        foreach (var sr in toSearch)
        {
            var searchResult = SearchRangeInAlmanacMap(map, sr);
            var unmatchedHead = searchResult[0];
            var matched = searchResult[1];
            var unmatchedTail = searchResult[2];

            if (unmatchedHead is not null)
                unmatched.Add(unmatchedHead);

            if(matched is not null)
                result.Add(matched);

            if(unmatchedTail is not null)
                unmatched.Add(unmatchedTail);
        }

        toSearch = unmatched;
        if (toSearch.Count == 0)
            break;
    }

    result.AddRange(toSearch);

    return result;
}

// var input = File.ReadLines("input.txt");
// using var enumerator = input.GetEnumerator();
// enumerator.MoveNext();
//
// // Parse seed line
// var seeds = enumerator.Current.Split(' ')[1..].Select(long.Parse).ToArray();
// var seedRanges = GetSeedRanges(seeds).ToArray();
// enumerator.MoveNext();
//
// var seedToSoilMapData = ReadMapData(enumerator).ToArray();
// var soilToFertilizerMapData = ReadMapData(enumerator).ToArray();
// var fertilizerToWaterMapData = ReadMapData(enumerator).ToArray();
// var waterToLightMapData = ReadMapData(enumerator).ToArray();
// var lightToTemperatureMapData = ReadMapData(enumerator).ToArray();
// var temperatureToHumidityMapData = ReadMapData(enumerator).ToArray();
// var humidityToLocationMapData = ReadMapData(enumerator).ToArray();
//
// var locations = seeds.Select(MapSeedToLocation).ToArray();
//
// var lowestLocation = locations.Min();
//
// Console.WriteLine(lowestLocation);
//

//
// long MapSeedToLocation(long seed)
// {
//     var soil = Map(seed, seedToSoilMapData);
//     var fertilizer = Map(soil, soilToFertilizerMapData);
//     var water = Map(fertilizer, fertilizerToWaterMapData);
//     var light = Map(water, waterToLightMapData);
//     var temperature = Map(light, lightToTemperatureMapData);
//     var humidity = Map(temperature, temperatureToHumidityMapData);
//     var location = Map(humidity, humidityToLocationMapData);
//
//     return location;
// }
//
//
// static IEnumerable<MapData> ReadMapData(IEnumerator<string> enumerator)
// {
//     enumerator.MoveNext();
//     enumerator.MoveNext();
//
//     while (!string.IsNullOrWhiteSpace(enumerator.Current))
//     {
//         var data = enumerator.Current.Split(' ').Select(long.Parse).ToArray();
//         yield return new MapData(data[0], data[1], data[2]);
//         enumerator.MoveNext();
//     }
// }

record MapData(long DestinationStart, long SourceStart, long Length);
record SeedRange(long Start, long End);
