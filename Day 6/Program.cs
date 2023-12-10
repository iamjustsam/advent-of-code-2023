// See https://aka.ms/new-console-template for more information

var input = File.ReadAllLines("input.txt");
var times = input[0].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToArray();
var distances = input[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToArray();

var result = 1d;

for (var i = 0; i < times.Length; i++)
{
    var raceTime = int.Parse(times[i]);
    var minDistance = int.Parse(distances[i]);

    var (minHoldTime, maxHoldTime) = CalculateMinAndMaxHoldTimeFromDistance(minDistance, raceTime);
    // var winningDistances = Enumerable.Range(0, raceTime).Select(x => CalculateDistance(x, raceTime)).Where(x => x > minDistance);
    var numberOfWinningDistances = Math.Floor(maxHoldTime) - Math.Ceiling(minHoldTime) + 1;

    result *= numberOfWinningDistances;
}

Console.WriteLine(result);

var singleTime = long.Parse(input[0].Split(':')[1].Replace(" ", null));
var singleDistance = long.Parse(input[1].Split(':')[1].Replace(" ", null));

var winners = Enumerable.Range(0, (int)singleTime).Select(x => CalculateDistance(x, singleTime)).Where(x => x > singleDistance);
var numberOfWinners = winners.Count();

Console.WriteLine(numberOfWinners);

return;

// x = holdtime
// y = distance
// t = total race time
// y = t * x - x^2

static long CalculateDistance(long holdTime, long totalRaceTime) => (totalRaceTime * holdTime) - (holdTime * holdTime);

static (double, double) CalculateMinAndMaxHoldTimeFromDistance(long distance, long totalRaceTime)
{
    var tmp1 = Math.Pow(totalRaceTime, 2) - 4 * distance;
    var tmp2 = Math.Sqrt(tmp1);

    var r1 = (totalRaceTime - tmp2) / 2;
    var r2 = (totalRaceTime + tmp2) / 2;

    return (r1, r2);
}
