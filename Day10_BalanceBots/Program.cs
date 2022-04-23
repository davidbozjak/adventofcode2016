using System.Text.RegularExpressions;
using System.Diagnostics;

Regex numRegex = new(@"\d+");

var instructionStrings = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();
var instructionsToExecute = new List<TransferInstruction>();

var chips = new UniqueFactory<int, Chip>(id => new Chip(id));
var bots = new UniqueFactory<int, Bot>(id => new Bot(id));
var outputs = new UniqueFactory<int, ChipOutput>(id => new ChipOutput(id));

foreach (var instruction in instructionStrings)
{
    var numbers = numRegex.Matches(instruction).Select(w => int.Parse(w.Value)).ToArray();

    if (instruction.Contains("value"))
    {
        if (numbers.Length != 2) throw new Exception();

        var bot = bots.GetOrCreateInstance(numbers[1]);
        bot.ReceiveChip(chips.GetOrCreateInstance(numbers[0]));
    }
    else if (instruction.Contains("gives"))
    {
        if (numbers.Length != 3) throw new Exception();

        var senderBot = bots.GetOrCreateInstance(numbers[0]);

        var instructionForLow = instruction[instruction.IndexOf("low")..instruction.IndexOf("and")];
        var instructionForHigh = instruction[instruction.IndexOf("and")..];

        ChipRecipient recipientLow = instructionForLow.Contains("bot") ? bots.GetOrCreateInstance(numbers[1]) : outputs.GetOrCreateInstance(numbers[1]);
        ChipRecipient recipientHigh = instructionForHigh.Contains("bot") ? bots.GetOrCreateInstance(numbers[2]) : outputs.GetOrCreateInstance(numbers[2]);

        instructionsToExecute.Add(new TransferInstruction(senderBot, recipientLow, recipientHigh));
    }
    else throw new Exception();
}

// DEBUG data
//var tracingChip1 = chips.GetOrCreateInstance(5);
//var tracingChip2 = chips.GetOrCreateInstance(2);

//Real puzzle data
var tracingChip1 = chips.GetOrCreateInstance(61);
var tracingChip2 = chips.GetOrCreateInstance(17);

while (instructionsToExecute.Count > 0)
{
    var instruction = instructionsToExecute.Where(w => w.Bot.IsReadyToTransfer).First();

    var bot = instruction.Bot;
    
    if (bot.ContainsChip(tracingChip1) && bot.ContainsChip(tracingChip2))
    {
        Console.WriteLine($"Part 1: Bot {bot.Id} is responsible for comparing Chip {tracingChip1.Id} and Chip {tracingChip2.Id}");
    }
    
    bot.TransferChips(instruction.LowerChipRecipient, instruction.HigherChipRecipient);

    instructionsToExecute.Remove(instruction);
}

Console.WriteLine($"Part 2: {outputs.GetOrCreateInstance(0).ValueOfFirstChip * outputs.GetOrCreateInstance(1).ValueOfFirstChip * outputs.GetOrCreateInstance(2).ValueOfFirstChip}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

[DebuggerDisplay("Chip {Id}")]
record Chip (int Id);

record TransferInstruction(Bot Bot, ChipRecipient LowerChipRecipient, ChipRecipient HigherChipRecipient);

abstract class ChipRecipient
{
    protected readonly List<Chip> chips = new();

    public int Id { get; }

    public ChipRecipient(int id)
    {
        this.Id = id;
    }

    public void ReceiveChip(Chip c)
    {
        this.chips.Add(c);
    }
}

[DebuggerDisplay("Bot {Id}")]
class Bot : ChipRecipient
{
    public bool IsReadyToTransfer => this.chips.Count == 2;

    public Bot(int id) : base(id)
    {
    }

    public bool ContainsChip(Chip c) =>
        this.chips.Contains(c);

    public void TransferChips(ChipRecipient setLower, ChipRecipient setHigher)
    {
        if (!this.IsReadyToTransfer) throw new Exception();

        setLower.ReceiveChip(this.chips.OrderBy(w => w.Id).First());
        setHigher.ReceiveChip(this.chips.OrderByDescending(w => w.Id).First());

        this.chips.Clear();
    }
}

[DebuggerDisplay("Output {Id}")]
class ChipOutput : ChipRecipient
{
    public ChipOutput(int id) : base(id)
    {
    }

    public int ValueOfFirstChip => this.chips[0].Id;
}