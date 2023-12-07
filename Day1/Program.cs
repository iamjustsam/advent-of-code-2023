// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

var numberRegex = new Regex("one|two|three|four|five|six|seven|eight|nine");
var rtlNumberRegex = new Regex("one|two|three|four|five|six|seven|eight|nine", RegexOptions.RightToLeft);
var numbers = new Dictionary<string, int>
{
    {"one", 1},
    {"two", 2},
    {"three", 3},
    {"four", 4},
    {"five", 5},
    {"six", 6},
    {"seven", 7},
    {"eight", 8},
    {"nine", 9}
};
var input = File.ReadAllLines("input.txt");
var output = input.Select(x =>
{
    var firstNumber = numberRegex.Replace(x, m => $"{numbers[m.Value]}", 1);
    var lastNumber = rtlNumberRegex.Replace(x, m => $"{numbers[m.Value]}", 1);

    return int.Parse($"{firstNumber.First(char.IsDigit)}{lastNumber.Last(char.IsDigit)}");
}).Sum();

Console.WriteLine(output);
