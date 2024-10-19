// See https://aka.ms/new-console-template for more information

var (rules, parts) = ParseInput(File.ReadAllText("input.txt"));
var acceptedParts = parts.Where(x => ValidatePart(rules, x));
var part1 = acceptedParts.Select(x => x.X + x.M + x.A + x.S).Sum();

Console.WriteLine(part1);

return;

static bool ValidatePart(IDictionary<string, Rule> rules, Part part)
{
    var rule = rules["in"];

    while (true)
    {
        var nextRuleId = string.Empty;
        foreach (var instruction in rule.Instructions)
        {
            var check = instruction.Operator switch
            {
                '>' => instruction.Left(part) > instruction.Right,
                '<' => instruction.Left(part) < instruction.Right,
                _ => throw new Exception("Invalid operator")
            };

            if (check)
            {
                nextRuleId = instruction.NextRuleId;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(nextRuleId))
            nextRuleId = rule.Fallback;

        switch (nextRuleId)
        {
            case "A":
                return true;
            case "R":
                return false;
        }

        rule = rules[nextRuleId];
    }
}

static (IDictionary<string, Rule>, Part[]) ParseInput(string input)
{
    var chunks = input.Split("\r\n\r\n");

    var rules = ParseRules(chunks[0].Split("\r\n").Where(x => !string.IsNullOrWhiteSpace(x)));
    var parts = ParseParts(chunks[1].Split("\r\n").Where(x => !string.IsNullOrWhiteSpace(x))).ToArray();

    return (rules, parts);
}

static IDictionary<string, Rule> ParseRules(IEnumerable<string> input)
{
    return input.Select(line =>
    {
        var chunks = line.TrimEnd('}').Split('{');
        var id = chunks[0];

        var instructionChunks = chunks[1].Split(',');
        var fallback = instructionChunks[^1];
        var instructions = instructionChunks[..^1].Select(x =>
        {
            var instructionData = x.Split(':');
            var left = GetLeftHandSelector(instructionData[0][0]);
            var op = instructionData[0][1];
            var right = int.Parse(instructionData[0][2..]);
            var nextRuleId = instructionData[1];
            return new Instruction(left, right, op, nextRuleId);
        });

        return new Rule(id, instructions.ToArray(), fallback);
    }).ToDictionary(x => x.Id);
}

static Func<Part, int> GetLeftHandSelector(char propertyName)
{
    return propertyName switch
    {
        'x' => part => part.X,
        'm' => part => part.M,
        'a' => part => part.A,
        's' => part => part.S,
        _ => throw new Exception("Invalid property name")
    };
}

static IEnumerable<Part> ParseParts(IEnumerable<string> input)
{
    return input.Select(line =>
    {
        var chunks = line.Trim('{', '}').Split(',');
        var x = int.Parse(chunks[0][2..]);
        var m = int.Parse(chunks[1][2..]);
        var a = int.Parse(chunks[2][2..]);
        var s = int.Parse(chunks[3][2..]);

        return new Part(x, m, a, s);
    });
}

internal record Rule(string Id, Instruction[] Instructions, string Fallback);
internal record Instruction(Func<Part, int> Left, int Right, char Operator, string NextRuleId);
internal record Part(int X, int M, int A, int S);
