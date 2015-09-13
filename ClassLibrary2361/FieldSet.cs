using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ThinkingClassLibary;
using System.Xml.Serialization;
using System.IO;
namespace ClassLibrary2361
{
    public class FieldSet : GenerationSet
    {
        public new List<Field> Generations;

        public static FieldSet NewField(BleepyBloop[] Olds)
        {
            List<Instruction[]> temp = new List<Instruction[]>();
            List<Instruction[]> clones = new List<Instruction[]>();
            List<Instruction> GenePool = new List<Instruction>();
            Olds = Olds.OrderByDescending(y => y.ObjectiveFunction()).ToArray();
            Field F = FillGenePool(Olds, clones, GenePool);
            AddPaddingToGenePool(GenePool);

            FillFromGenePool(temp, GenePool);
            F.Bloops = new InTime<BleepyBloop>[temp.Count() + clones.Count()];
            AddBloopsToField(temp, clones, F);
            F = Field.GenerateNewLevel(F.Bloops.Select(t => t.AtFrame[0]).ToArray());
            return FieldSetFromField(F);
        }

        private static void FillFromGenePool(List<Instruction[]> temp, List<Instruction> GenePool)
        {
            int NumberOfBloops = GenePool.Count / BleepyBloop.MemSize;
            for (int i = 0; i < NumberOfBloops; i++)
            {
                temp.Add(GenePool.Take(BleepyBloop.MemSize).ToArray());
                GenePool.RemoveRange(0, BleepyBloop.MemSize);
            }
        }

        private static void AddBloopsToField(List<Instruction[]> temp, List<Instruction[]> clones, Field F)
        {
            for (int i = 0; i < temp.Count(); i++)
            {
                F.Bloops[i] = new InTime<BleepyBloop>();
                F.Bloops[i].AtFrame = new List<BleepyBloop>();
                F.Bloops[i].AtFrame.Add(new BleepyBloop());
                F.Bloops[i].AtFrame[0].Genes = temp[i];
                F.Bloops[i].AtFrame[0].Vary(0.08, 0.08);
            }
            for (int i = 0; i < clones.Count(); i++)
            {
                int b = i + temp.Count;
                F.Bloops[b] = new InTime<BleepyBloop>();
                F.Bloops[b].AtFrame = new List<BleepyBloop>();
                F.Bloops[b].AtFrame.Add(new BleepyBloop());
                F.Bloops[b].AtFrame[0].Genes = clones[i];
            }
        }

        private static void AddPaddingToGenePool(List<Instruction> GenePool)
        {
            for (int i = 0; i < BleepyBloop.MemSize * 32; i++)
            {
                GenePool.Add(new Instruction(BleepyBloop.MemSize,BleepyBloop.InputSize,BleepyBloop.OutputSize));
            }
        }

        private static Field FillGenePool(BleepyBloop[] Olds, List<Instruction[]> Clones, List<Instruction> GenePool)
        {
            Field F = new Field(Olds.Count());
            double Mean = Olds.Sum(x => GetNumberOfGenesToPassFromBloop(x)) / 32;
            foreach (BleepyBloop b in Olds)
            {
                ExtractGenesToPassOn(Clones, GenePool, Mean, b);
            }

            return F;
        }

        private static void ExtractGenesToPassOn(List<Instruction[]> Clones, List<Instruction> GenePool, double Mean, BleepyBloop b)
        {
            int NumberOfInstructionsToPassOn;
            double v = GetNumberOfGenesToPassFromBloop(b);
            if (v >= Mean)
            {
                Clones.Add(b.Genes);
                if (v >= (Mean * 2))
                {
                    Clones.Add(b.ReturnVaried(0.08, 0.08));
                    NumberOfInstructionsToPassOn = (int)Math.Round(BleepyBloop.MemSize * (v / Mean - 2));
                }
                else
                {
                    NumberOfInstructionsToPassOn = (int)Math.Round(BleepyBloop.MemSize * (v / Mean - 1));
                }
            }
            else
            {
                NumberOfInstructionsToPassOn = (int)Math.Round(BleepyBloop.MemSize * (v / Mean));
            }
            FillGenePoolWithBloopsGenes(GenePool, b, NumberOfInstructionsToPassOn);
        }

        private static void FillGenePoolWithBloopsGenes(List<Instruction> GenePool, BleepyBloop b, int NumberOfInstructionsToPassOn)
        {
            for (int j = 0; j < -NumberOfInstructionsToPassOn; j--)
            {
                GenePool.Add(b.Genes[j % BleepyBloop.MemSize]);
            }
        }

        private static double GetNumberOfGenesToPassFromBloop(BleepyBloop b)
        {
            return Math.Pow(b.ObjectiveFunction(),1.5);
        }

        public static FieldSet NewField(int Number)
        {
            Field F = NewFieldWithRandomBloops(Number);

            return FieldSetFromField(F);
        }

        private static Field NewFieldWithRandomBloops(int Number)
        {
            Field F = new Field(Number);
            F.Bloops = new InTime<BleepyBloop>[Number];
            F.bleepSim.Bloops = F.Bloops;
            F.physicalLevel.Foods = new List<Food>();
            F.physicalLevel.Poisons = new List<Poison>();
            for (int i = 0; i < Number; i++)
            {
                F.Bloops[i] = new InTime<BleepyBloop>();
                F.Bloops[i].AtFrame.Add(new BleepyBloop());
            }
            F.physicalLevel.AddNewFoodAndPoison(48);
            F.physicalLevel.UpdateThingChache(F);
            F.bleepSim.SetThreads(BleepSim.NumThread,Field.NF);
            return F;
        }

        private static FieldSet FieldSetFromField(Field F)
        {
            FieldSet S = new FieldSet();
            S.Generations = new List<Field>();
            S.Generations.Add(F);
            return S;
        }
    }
}
