using System.Diagnostics;

class Computer
{
    private readonly UniqueFactory<string, Register> Registers;
    private readonly List<string> InstructionStrings;
    private readonly Func<long, bool> handleOutputFunc;
    private int instructionIndex;

    public Computer(IEnumerable<string> instructions, Func<long, bool> handleOutputFunc)
    {
        this.InstructionStrings = instructions.ToList();
        this.handleOutputFunc = handleOutputFunc;
        this.Registers = new UniqueFactory<string, Register>(name => new Register(name));
        this.instructionIndex = 0;
    }

    public void SetRegisterValue(string register, long value)
    {
        var r = Registers.GetOrCreateInstance(register);
        r.Value = value;
    }

    public long GetRegisterValue(string register)
    {
        var r = Registers.GetOrCreateInstance(register);
        return r.Value;
    }

    public void Run()
    {
        for (; instructionIndex >= 0 && instructionIndex < InstructionStrings.Count; instructionIndex++)
        {
            var instructionString = InstructionStrings[instructionIndex];

            var parts = instructionString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var instruction = parts[0];

            if (instruction == "cpy")
            {
                var r = Registers.GetOrCreateInstance(parts[2]);

                var value = GetValueOrRegisterValue(parts[1]);

                r.Value = value;
            }
            else if (instruction == "inc")
            {
                var r = Registers.GetOrCreateInstance(parts[1]);

                r.Value++;
            }
            else if (instruction == "dec")
            {
                var r = Registers.GetOrCreateInstance(parts[1]);

                r.Value--;
            }
            else if (instruction == "jnz")
            {
                var valueX = GetValueOrRegisterValue(parts[1]);

                if (valueX != 0)
                {
                    var valueY = GetValueOrRegisterValue(parts[2]);

                    instructionIndex += (int)(valueY - 1);
                }
            }
            else if (instruction == "out")
            {
                var value = GetValueOrRegisterValue(parts[1]);

                var shouldExecutionContinue = this.handleOutputFunc(value);

                if (!shouldExecutionContinue) break;
            }
            else throw new Exception("Unknown instruction");
        }

        long GetValueOrRegisterValue(string valueOrRegister)
        {
            if (!long.TryParse(valueOrRegister, out long value))
            {
                var rx = Registers.GetOrCreateInstance(valueOrRegister);
                value = rx.Value;
            }

            return value;
        }
    }

    [DebuggerDisplay("{Name}:{Value}")]
    protected class Register
    {
        public string Name { get; }
        public long Value { get; set; } = 0;

        public Register(string name)
        {
            this.Name = name;
        }
    }
}