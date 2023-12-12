// See https://aka.ms/new-console-template for more information

var input = File.ReadLines("input.txt").Select(x => x.Split(' ').Select(int.Parse).Reverse().ToArray());

var total = 0;
foreach (var sequence in input)
{
    // Step 1: Build sequences
    var sequences = new List<List<int>>();
    var seq = sequence.ToList();
    sequences.Add(seq);

    while (seq.Any(x => x != 0))
    {
        var newSeq = new List<int>();
        for (var i = 1; i < seq.Count; i++)
        {
            newSeq.Add(seq[i] - seq[i - 1]);
        }

        sequences.Add(newSeq);
        seq = newSeq;
    }

    // Step 2: Use built sequences to calculate next value
    sequences[^1].Add(0);
    for (var i = 0; i < sequences.Count - 1; i++)
    {
        var prevSeq = sequences[^(i + 1)];
        var currSeq = sequences[^(i + 2)];

        currSeq.Add(currSeq[^1] + prevSeq[^1]);
    }

    total += sequences[0][^1];
}

Console.WriteLine(total);
