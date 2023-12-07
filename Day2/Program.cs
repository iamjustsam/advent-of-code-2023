// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

var gameIdRegex = new Regex("^Game (\\d+):.*$");
var redCubesRegex = new Regex("(\\d+) red");
var greenCubesRegex = new Regex("(\\d+) green");
var blueCubesRegex = new Regex("(\\d+) blue");

const int maxRed = 12;
const int maxGreen = 13;
const int maxBlue = 14;

var input = File.ReadLines("input.txt");

var games = input.Select(x =>
{
    var gameId = int.Parse(gameIdRegex.Match(x).Groups[1].Value);
    var gameSets = x.Split(';').Select(gameSetInput =>
    {
        var redCubeCount = redCubesRegex.Match(gameSetInput).Groups[1].Value;
        var greenCubeCount = greenCubesRegex.Match(gameSetInput).Groups[1].Value;
        var blueCubeCount = blueCubesRegex.Match(gameSetInput).Groups[1].Value;

        return new GameSet(ParseInt(redCubeCount), ParseInt(greenCubeCount), ParseInt(blueCubeCount));
    }).ToArray();

    return new GameInfo(gameId, gameSets);
}).ToArray();

var possibleGames = games.Where(x => x.Sets.Max(y => y.RedCount) <= maxRed && x.Sets.Max(y => y.GreenCount) <= maxGreen && x.Sets.Max(y => y.BlueCount) <= maxBlue);
var output1 = possibleGames.Sum(x => x.Id);

var minimumSetPowers = games.Select(x =>
{
    var minRed = x.Sets.Max(y => y.RedCount);
    var minGreen = x.Sets.Max(y => y.GreenCount);
    var minBlue = x.Sets.Max(y => y.BlueCount);

    return minRed * minGreen * minBlue;
});
var output2 = minimumSetPowers.Sum();

Console.WriteLine($"Sum of possible games: {output1}");
Console.WriteLine($"Sum of powers: {output2}");

return;

static int ParseInt(string input) => string.IsNullOrWhiteSpace(input) ? 0 : int.Parse(input);

record GameInfo(int Id, GameSet[] Sets);
record GameSet(int RedCount, int GreenCount, int BlueCount);
