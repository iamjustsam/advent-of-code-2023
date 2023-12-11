// See https://aka.ms/new-console-template for more information

var input = File.ReadLines("input.txt")
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(LineToHand)
    .OrderBy(x => x, new CamelCardHandComparer())
    .ToArray();

var totalWinnings = input.Select((x, i) => x.Bid * (i + 1)).Sum();

Console.WriteLine(totalWinnings);

return;

static CamelCardHand LineToHand(string line)
{
    var chunks = line.Split(' ');
    var cards = chunks[0].Select(CardToValue).ToArray();
    var bid = int.Parse(chunks[1]);
    var type = GetHandType(cards);

    return new CamelCardHand(chunks[0], type, cards, bid);
}

static int CardToValue(char card) =>
    card switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 1,
        'T' => 10,
        _ => int.Parse(card.ToString())
    };

static HandType GetHandType(int[] cards)
{
    var groups = cards.Where(x => x != 1).GroupBy(x => x).OrderByDescending(y => y.Count()).ToArray();
    var handType = HandType.HighCard;

    foreach (var group in groups)
    {
        if (group.Count() == 5)
        {
            handType = HandType.FiveOfAKind;
            break;
        }

        if (group.Count() == 4)
        {
            handType = HandType.FourOfAKind;
            break;
        }

        if (group.Count() == 3 && groups.Any(x => x.Count() == 2))
        {
            handType = HandType.FullHouse;
            break;
        }

        if (group.Count() == 3)
        {
            handType = HandType.ThreeOfAKind;
            break;
        }

        if (group.Count() == 2 && groups.Count(x => x.Count() == 2) == 2)
        {
            handType = HandType.TwoPair;
            break;
        }

        if (group.Count() == 2)
        {
            handType = HandType.OnePair;
            break;
        }
    }

    var numberOfJokers = cards.Count(x => x == 1);

    return Upgrade(handType, numberOfJokers);
}

static HandType Upgrade(HandType handType, int numberOfJokers)
{
    return handType switch
    {
        // 5 possible jokers
        HandType.HighCard when numberOfJokers is 1 => HandType.OnePair,
        HandType.HighCard when numberOfJokers is 2 => HandType.ThreeOfAKind,
        HandType.HighCard when numberOfJokers is 3 => HandType.FourOfAKind,
        HandType.HighCard when numberOfJokers is 4 => HandType.FiveOfAKind,
        HandType.HighCard when numberOfJokers is 5 => HandType.FiveOfAKind,
        // 3 possible jokers
        HandType.OnePair when numberOfJokers is 1 => HandType.ThreeOfAKind,
        HandType.OnePair when numberOfJokers is 2 => HandType.FourOfAKind,
        HandType.OnePair when numberOfJokers is 3 => HandType.FiveOfAKind,
        // 1 possible joker
        HandType.TwoPair when numberOfJokers is 1 => HandType.FullHouse,
        // 2 possible jokers
        HandType.ThreeOfAKind when numberOfJokers is 1 => HandType.FourOfAKind,
        HandType.ThreeOfAKind when numberOfJokers is 2 => HandType.FiveOfAKind,
        // 1 possible joker
        HandType.FourOfAKind when numberOfJokers is 1 => HandType.FiveOfAKind,
        _ => handType
    };
}

record CamelCardHand(string Raw, HandType Type, int[] Cards, int Bid);

enum HandType
{
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    FullHouse = 5,
    FourOfAKind = 6,
    FiveOfAKind = 7
}

class CamelCardHandComparer : IComparer<CamelCardHand>
{
    public int Compare(CamelCardHand? x, CamelCardHand? y)
    {
        switch (x)
        {
            case null when y is not null:
                return -1;
            case null when y is null:
                return 0;
            case not null when y is null:
                return 1;
        }
        /*
         * Value
Meaning
Less than zero
x is less than y.
Zero
x equals y.
Greater than zero
x is greater than y.
         */

        if (x!.Type < y.Type)
            return -1;

        if (x.Type > y.Type)
            return 1;

        // Get the first card that is not equal
        for (var i = 0; i < x.Cards.Length; i++)
        {
            var xCard = x.Cards[i];
            var yCard = y.Cards[i];

            if(xCard == yCard)
                continue;

            return xCard.CompareTo(yCard);
        }

        return 0;
    }
}
