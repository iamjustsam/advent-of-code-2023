// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Text.RegularExpressions;

var cardIdRegex = new Regex(@"^Card\s+(\d+):.*$");
var input = File.ReadAllLines("input.txt");
var scratchCards = GetScratchCards(input).ToArray();

var output1 = scratchCards.Aggregate(0, (total, card) => total + (card.NumberOfMatches > 0 ? 1 << card.NumberOfMatches - 1 : 0));
var output2 = ScratchAll(scratchCards).Count();

Console.WriteLine(output1);
Console.WriteLine($"{output2}");

// foreach (var scratchCard in input)
// {
//     var gameInput = scratchCard.Split(':');
//     var numbers = gameInput[1].Trim().Split('|');
//     var winningNumbers = numbers[0].Trim().Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
//     var ownNumbers = numbers[1].Trim().Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
//     var numberOfMatches = ownNumbers.Count(x => winningNumbers.Contains(x));
//
//     sum += numberOfMatches > 0 ? 1 << numberOfMatches - 1 : 0;
// }
//
// Console.WriteLine(sum);

static IEnumerable<ScratchCard> ScratchAll(ScratchCard[] cards)
{
    var dict = cards.ToDictionary(x => x.CardNumber);
    var cardQueue = new Queue<ScratchCard>(cards);

    while (cardQueue.Any())
    {
        var card = cardQueue.Dequeue();
        var cardNumber = card.CardNumber;
        var numberOfMatches = card.NumberOfMatches;

        for (int i = 1; i <= numberOfMatches; i++)
        {
            var cardToCopy = dict[cardNumber + i];
            cardQueue.Enqueue(cardToCopy);
        }

        yield return card;
    }
}

IEnumerable<ScratchCard> GetScratchCards(IEnumerable<string> data)
{
    foreach (var scratchCard in data)
    {
        if(string.IsNullOrWhiteSpace(scratchCard))
            continue;

        var cardId = int.Parse(cardIdRegex.Match(scratchCard).Groups[1].Value);
        var gameInput = scratchCard.Split(':');
        var numbers = gameInput[1].Trim().Split('|');
        var winningNumbers = numbers[0].Trim().Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var ownNumbers = numbers[1].Trim().Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var numberOfMatches = ownNumbers.Count(x => winningNumbers.Contains(x));

        yield return new ScratchCard(cardId, winningNumbers, ownNumbers, numberOfMatches);
    }
}

record ScratchCard(int CardNumber, string[] WinningNumbers, string[] OwnNumbers, int NumberOfMatches);
