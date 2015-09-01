﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ThinkingClassLibary
{
    public class Instruction
    {
        public MemAdr InAdrA;
        public MemAdr HyAdrB;
        public MemAdr OutAdr;
        public TwoCalculation Instuct;
        
        public object InstructAsItsType
        {
            get
            {
                return (Instuct);
            }
        }

        public Instruction(int MemSize)
        {
            InAdrA = RandomAddress(true, true, false, MemSize, 12, 2);
            HyAdrB = RandomAddress(true, true, false, MemSize, 12, 2);
            OutAdr = RandomAddress(true, false, true, MemSize, 12, 2);
            Instuct = Calcuations.GetRandom();
        }

        public Instruction()
        {
            int DefaultSize = 256;
            InAdrA = RandomAddress(true, true, false, DefaultSize, 12, 2);
            HyAdrB = RandomAddress(true, true, false, DefaultSize, 12, 2);
            OutAdr = RandomAddress(true, false, true, DefaultSize, 12, 2);
            Instuct = Calcuations.GetRandom();
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


}