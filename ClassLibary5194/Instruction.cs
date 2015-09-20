namespace ThinkingClassLibary
{
    public class InstructionBase
    {
        public TwoCalculation Instuct;

        public InstructionBase()
        { 
        }

        public object InstructAsItsType
        {
            get
            {
                return (Instuct);
            }
        }
    }

    public class Instruction : InstructionBase
    {
        public MemAdr InAdrA;
        public MemAdr HyAdrB;
        public MemAdr OutAdr;

        public Instruction(int MemSize, int InputSize, int OutputSize)
        {
            InAdrA = RandomAddress(true, true, false, MemSize, InputSize, OutputSize);
            HyAdrB = RandomAddress(true, true, false, MemSize, InputSize, OutputSize);
            OutAdr = RandomAddress(true, false, true, MemSize, InputSize, OutputSize);
            Instuct = Calcuations.GetRandom();
        }

        public Instruction() : base()
        {
        }

        public Instruction(Instruction i)
        {
            InAdrA = i.InAdrA;
            HyAdrB = i.HyAdrB;
            OutAdr = i.OutAdr;
            Instuct = i.Instuct;
        }

        public static MemAdr RandomAddress(bool CanBeMemory, bool CanBeInput, bool CanBeOutput, int MemSize, int InputSize, int OutputSize)
        {
            MemAdr Ve = new MemAdr();
            int PossibleCombos = CountPossibleCombos(CanBeMemory, CanBeInput, CanBeOutput);
            ChooseTypeOfAddr(ref CanBeMemory, ref CanBeInput, ref CanBeOutput, PossibleCombos);
            GetAdrFromType(CanBeMemory, CanBeInput, MemSize, InputSize, OutputSize, Ve);

            return Ve;
        }

        private static void GetAdrFromType(bool CanBeMemory, bool CanBeInput, int MemSize, int InputSize, int OutputSize, MemAdr Ve)
        {
            if (CanBeMemory)
            {
                Ve.T = 0;
                Ve.P = BetterRandom.R.Next(MemSize);
            }
            else if (CanBeInput)
            {
                Ve.T = 1;
                Ve.P = BetterRandom.R.Next(InputSize);
            }
            else
            {
                Ve.T = 2;
                Ve.P = BetterRandom.R.Next(OutputSize);
            }
        }

        private static void ChooseTypeOfAddr(ref bool CanBeMemory, ref bool CanBeInput, ref bool CanBeOutput, int PossibleCombos)
        {
            int AddrType = BetterRandom.R.Next(PossibleCombos);
            while (AddrType != 0)
            {
                if (CanBeMemory)
                {
                    CanBeMemory = false;
                    AddrType--;
                }
                else if (CanBeInput)
                {
                    CanBeInput = false;
                    AddrType--;
                }
                else
                {
                    CanBeOutput = false;
                    AddrType--;
                }
            }
        }

        private static int CountPossibleCombos(bool CanBeMemory, bool CanBeInput, bool CanBeOutput)
        {
            int PossibleCombos = 0;
            if (CanBeInput) PossibleCombos++;
            if (CanBeMemory) PossibleCombos++;
            if (CanBeOutput) PossibleCombos++;
            return PossibleCombos;
        }
    }

    public class CompiledInstruction:InstructionBase
    {
        public int InAdrA;
        public int HyAdrB;
        public int OutAdr;

        public CompiledInstruction(Instruction Instr, int MemSize, int InputSize)
        {
            InAdrA = ConvertMemAdrToInt(Instr.InAdrA, MemSize, InputSize);
            HyAdrB = ConvertMemAdrToInt(Instr.HyAdrB, MemSize, InputSize);
            OutAdr = ConvertMemAdrToInt(Instr.OutAdr, MemSize, InputSize);
            Instuct = Instr.Instuct;
        }

        public int ConvertMemAdrToInt(MemAdr Adr, int MemSize,int InputSize)
        {
            int TBR = Adr.P;
            if (Adr.T == 1)
            {
                TBR += MemSize;
            }
            else if (Adr.T == 2)
            {
                TBR += MemSize + InputSize;
            }
            return TBR;
        }
    }
}
