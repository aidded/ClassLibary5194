using System;
using System.Collections.Generic;
using System.Linq;
using ThinkingClassLibary;
using PhysicalSimulations;

namespace ClassLibrary2361
{
    public class BleepyBloop : LifeForm, IPhysical<Vector2d>
    {
        public Vector2d pos;

        public Vector2d Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }
        public ColourVector Colour
        {
            get
            {
                return (new ColourVector(0d, 1d, 0d, 0d));
            }
        }

        public ColourVector L;
        public ColourVector R;
        public ColourVector F;

        public override Instruction[] Genes
        {
            get
            {
                return G;
            }

            set
            {
                G = value;
            }
        }

        List<Instruction> genes
        {
            get
            {
                return G.ToList();
            }
            set
            {
                G = value.ToArray();
            }
        }

        public double Food
        {
            get
            {
                return food;
            }

            set
            {
                food = value;
            }
        }

        public Instruction[] G;

        public double Rotation;
        public Vector2d PosSensor(double LeftRight, double Forward)
        {
            return (new Vector2d
                (pos.x +
                (Forward* Math.Sin(Rotation)) +
                (0.4 * LeftRight * Math.Cos(Rotation)),
                pos.y + (Forward * Math.Cos(Rotation)) +
                (0.4 * LeftRight * Math.Sin(Rotation))));
        }

        private double food=15;
        public double Poison = -1;
        public double MoveSpeed
        {
            get
            {
                if (Poison < 0)
                {
                    return 0.4;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double[] Inputs;
        public enum IAL : int
        {
            InGreenL = 0,
            InGreenR = 1,
            InGreenF = 2,
            InOrangeL =3,
            InOrangeR = 4,
            InOrangeF = 5,
            InGreyL = 6,
            InGreyR = 7,
            InGreyF = 8,
            InBlueL = 9,
            InBlueR = 10,
            InBlueF = 11,
            ConstOne = 12,
            ConstHalf = 13,
            ConstZero = 14,
            ConstNegativeOne = 15,
            ValuInBuferOut1 = 16,
            ValuInBuferOut2 = 17,
            ValuInBuferOut3 = 18,
            ValuInBuferOut4 = 19

        }
        public static int InputSize = 24;
        public double[] Memory;
        public static int MemSize = 750;
        public double[] Outputs;
        public static int OutputSize = 14;
        public enum OAL:int
        {
            OutThrustL = 0,
            OutThrustR = 1,
            ValuToPutInBuffer1 = 2,
            ToBufferInputAddr1 = 3,
            FromBufrOutputAdr1 = 4,
            ValuToPutInBuffer2 = 5,
            ToBufferInputAddr2 = 6,
            FromBufrOutputAdr2 = 7,
            ValuToPutInBuffer3 = 8,
            ToBufferInputAddr3 = 9,
            FromBufrOutputAdr3 = 10,
            ValuToPutInBuffer4 = 11,
            ToBufferInputAddr4 = 12,
            FromBufrOutputAdr4 = 13,
        }

        public static int NumMemBuffet = 4;
        public static int MemBufSize = 35000;
        public MemoryBuffet[] Buffet;

        public override double ObjectiveFunction()
        {
            return Food;
        }

        public void Vary(double StdDev, double p)
        {
            Genes = ReturnVaried(StdDev, p);
        }

        public Instruction[] ReturnVaried(double StdDev, double p)
        {
            BleepyBloop b = new BleepyBloop();
            b = (BleepyBloop)this;
            for (int i = 0; i < genes.Count(); i++)
            {
                Instruction instruction = new Instruction(genes[i]);
                TwoCalculation calc = instruction.Instuct;
                calc.PropertyA = Clamper.clamp(calc.PropertyA + BetterRandom.StdDev(StdDev));
                calc.PropertyB = Clamper.clamp(calc.PropertyB + BetterRandom.StdDev(StdDev));

                if (BetterRandom.NextDouble() < p)
                {
                    instruction.InAdrA = Instruction.RandomAddress(true, true, false, MemSize, BleepyBloop.InputSize, OutputSize);
                }
                if (BetterRandom.NextDouble() < p)
                {
                    instruction.HyAdrB = Instruction.RandomAddress(true, true, false, MemSize, BleepyBloop.InputSize, OutputSize);
                }
                if (BetterRandom.NextDouble() < p)
                {
                    instruction.OutAdr = Instruction.RandomAddress(true, false, true, MemSize, BleepyBloop.InputSize, OutputSize);
                }
                if (BetterRandom.NextDouble() < p)
                {
                    calc = Calcuations.GetRandom();
                }
                b.genes[i] = instruction;
            }
            return b.Genes;
        }


        public double GetAddr(MemAdr a)
        {
            if (a.bT)
            {
                return Inputs[a.P];
            }
            else
            {
                return Memory[a.P];
            }
        }

        public void SetAddr(MemAdr a, double d)
        {
            if(a.T == 0)
            {
                Memory[a.P] = d;
            }
            else
            {
                Outputs[a.P] = d;
            }
        }

        public BleepyBloop()
        {
            InitVars();
            Position = new Vector2d(BetterRandom.NextDouble() * 64, BetterRandom.NextDouble() * 64);
            for (int e = 0; e < MemSize; e++)
            {
                G[e] = new Instruction(MemSize,InputSize,OutputSize);
            }
        }

        private void InitVars()
        {
            Memory = new double[MemSize];
            Buffet = new MemoryBuffet[NumMemBuffet];
            for (int i = 0; i < Buffet.Count(); i++)
            {
                Buffet[i] = new MemoryBuffet(MemBufSize);
            }
            Inputs = new double[InputSize];
            Outputs = new double[OutputSize];
            L = new ColourVector();
            R = new ColourVector();
            F = new ColourVector();
            G = new Instruction[MemSize];
        }
    }
}
