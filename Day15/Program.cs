// See https://aka.ms/new-console-template for more information

var input = File.ReadAllText("input.txt").Trim().Split(',');
var hashes = input.Select(Hash);
var totalPart1 = hashes.Sum();

var boxes = new List<List<Lens>>(256);
boxes.AddRange(Enumerable.Range(0, 256).Select(_ => new List<Lens>()));

foreach (var operation in input)
{
    var label = operation.Split('-', '=')[0];
    var boxNumber = Hash(label);
    var boxLensList = boxes[boxNumber];
    var existingLens = boxLensList.SingleOrDefault(x => x.Label == label);

    if (existingLens != null && operation.Contains('-'))
    {
        boxLensList.Remove(existingLens);
    }
    else if(operation.Contains('='))
    {
        var lensToAdd = operation[^1];
        var focalLength = lensToAdd - '0';

        if (existingLens != null)
        {
            var existingLensIndex = boxLensList.IndexOf(existingLens);
            boxLensList[existingLensIndex] = existingLens with { FocalLength = focalLength };
        }
        else
        {
            boxLensList.Add(new Lens(label, focalLength));
        }
    }
}

var totalPart2 = boxes.Select((box, boxNumber) => box.Select((lens, slotNumber) => (1 + boxNumber) * (slotNumber + 1) * lens.FocalLength).Sum()).Sum();

Console.WriteLine(totalPart1);
Console.WriteLine(totalPart2);

return;

static int Hash(string input) => input.ToCharArray().Aggregate(0, (total, current) => (total + current) * 17 % 256);

record Lens(string Label, int FocalLength);
