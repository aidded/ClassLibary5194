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
            List<double> PastFoods = new List<double>();
            List<Instruction> GenePool = new List<Instruction>();
            Olds = Olds.OrderByDescending(y => y.ObjectiveFunction()).ToArray();
            Field F = new Field(Olds.Count());
            double Mean = FillGenePool(Olds, clones, GenePool,PastFoods);
            AddPaddingToGenePool(GenePool);

            FillFromGenePool(temp, GenePool);
            F.Bloops = new BleepyBloop[temp.Count() + clones.Count()];
            AddBloopsToField(temp, clones, F,Mean,PastFoods);
            F = Field.GenerateNewLevel(F.Bloops.ToArray());
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

        private static void AddBloopsToField(List<Instruction[]> temp, List<Instruction[]> clones, Field F,double Mean,List<double> PastFoods)
        {
            for (int i = 0; i < temp.Count(); i++)
            {
                BleepyBloop B = F.Bloops[i];
                B = new BleepyBloop();
                B.Genes = temp[i];
                B.Vary(0.08, 0.08);
                B.parentsFood = Mean;
                F.Bloops[i] = B;
            }
            for (int i = 0; i < clones.Count(); i++)
            {
                int b = i + temp.Count;
                F.Bloops[b] = new BleepyBloop();
                F.Bloops[b].Genes = clones[i];
                F.Bloops[b].parentsFood = PastFoods[i];
            }
        }

        private static void AddPaddingToGenePool(List<Instruction> GenePool)
        {
            for (int i = 0; i < BleepyBloop.MemSize * 32; i++)
            {
                GenePool.Add(new Instruction(BleepyBloop.MemSize,BleepyBloop.InputSize,BleepyBloop.OutputSize));
            }
        }

        private static double FillGenePool(BleepyBloop[] Olds, List<Instruction[]> Clones, List<Instruction> GenePool,List<double> PastFoods)
        {
            double Mean = Olds.Sum(x => GetNumberOfGenesToPassFromBloop(x)) / 32;
            double Total = Mean / 32;
            foreach (BleepyBloop b in Olds.OrderByDescending(b=>b.ObjectiveFunction()))
            {
                ExtractGenesToPassOn(Clones, GenePool, Mean, b, PastFoods,Total);
            }
            return Mean;
        }

        private static void ExtractGenesToPassOn(List<Instruction[]> Clones, List<Instruction> GenePool, double Mean, BleepyBloop b,List<double> PastFoods,double Total)
        {
            if (Total >= 0)
            {
                double v = Math.Round(GetNumberOfGenesToPassFromBloop(b));
                int NumberOfInstructionsToPassOn = (int)Math.Round(BleepyBloop.MemSize * (v / Mean));
                if (v >= Mean)
                {
                    Clones.Add(b.Genes);
                    PastFoods.Add(b.Food);
                    int NumbClones = 1;
                    if (v >= (Mean * 2))
                    {
                        NumbClones = AddAnotherClone(Clones, b, PastFoods, NumbClones);
                    }
                    NumberOfInstructionsToPassOn -= BleepyBloop.MemSize * NumbClones;
                    Total -= NumbClones*BleepyBloop.MemSize;
                }
                FillGenePoolWithBloopsGenes(GenePool, b, NumberOfInstructionsToPassOn);
                Total -= NumberOfInstructionsToPassOn;
            }
        }

        private static int AddAnotherClone(List<Instruction[]> Clones, BleepyBloop b, List<double> PastFoods, int NumbClones)
        {
            Clones.Add(b.ReturnVaried(0.08, 0.08));
            PastFoods.Add(b.Food);
            NumbClones++;
            return NumbClones;
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
            F.Bloops = new BleepyBloop[Number];
            F.bleepSim.Bloops = F.Bloops;
            F.physicalLevel.Foods = new List<Food>();
            F.physicalLevel.Poisons = new List<Poison>();
            for (int i = 0; i < Number; i++)
            {
                F.Bloops[i] = new BleepyBloop();
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
