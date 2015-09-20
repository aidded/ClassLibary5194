using PhysicalSimulations;
using System;
using System.Collections.Generic;
using System.Linq;
using ThinkingClassLibary;

namespace ClassLibrary2361
{
    public class Field : Generational
    {
        public PhysicalLevel physicalLevel;
        public BleepSim bleepSim;

        public BleepyBloop[] Bloops;
        public BleepyBloop[] PreviousOrderCheck;

        public const int NF = 25000;

        public Field(int NumberOfBloops)
        {
            PreviousOrderCheck = new BleepyBloop[0];
            physicalLevel = new PhysicalLevel();
            bleepSim = new BleepSim();
            InitRockBarrier();
        }

        private void InitRockBarrier()
        {
            physicalLevel.Rocks = new List<Rock>();
            double s = 80;
            for (double r = 0; r < (Math.PI * 2); r += 0.024)
            {
                physicalLevel.Rocks.Add(new Rock((Math.Sin(r) * s) + 50, (Math.Cos(r) * s) + 50));
            }
            for (int i = 0; i < 64; i++)
            {
                physicalLevel.Rocks.Add(new Rock());
            }
        }

        public Field()
        {
        }

        public new int Frame
        {
            get
            {
                return frame;
            }
            set
            {
                frame = bleepSim.Frame = value;
            }
        }

        public int frame;

        public override LifeForm[] Lifes
        {
            get
            {
                return (Bloops.ToList().ConvertAll(q=>(LifeForm)q).ToArray());
            }
        }

        public static Field GenerateNewLevel(BleepyBloop[] Olds)
        {
            Field F = InitNewFieldWithOldBleeps(Olds);
            F.physicalLevel.AddNewFoodAndPoison(48);
            F.physicalLevel.UpdateThingChache(F);
            F.bleepSim.Bloops = F.Bloops;
            F.bleepSim.SetThreads(BleepSim.NumThread,NF);
            return F;
        }

        public void MoveFromBrainToPhy(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                BleepyBloop h = Bloops[i];
                double V = h.MoveSpeed;
                var LThrust = Clamper.clamp(h.Outputs[(int)BleepyBloop.OAL.OutThrustL]);
                var RThrust = Clamper.clamp(h.Outputs[(int)BleepyBloop.OAL.OutThrustR]);
                double T = V * (LThrust + RThrust);
                h.pos.x += T * Math.Sin(h.Rotation);
                h.pos.y += T * Math.Cos(h.Rotation);
                if (double.IsNaN(h.pos.x)) Console.Beep();
                if (double.IsNaN(h.pos.y)) Console.Beep();
                h.Rotation += (RThrust - LThrust) / 5d;
            }
        }

        public void MoveFromPhyToBrain(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                BleepyBloop h = Bloops[i];
                if ((Frame % 4) == 0)
                {
                    h.L = new ColourVector();
                    h.R = new ColourVector();
                    h.F = new ColourVector();
                    physicalLevel.CAP(h.PosSensor(1d, 0.8), Frame, ref h.L);
                    physicalLevel.CAP(h.PosSensor(-1d, 0.8), Frame, ref h.R);
                    physicalLevel.CAP(h.PosSensor(0, 1.6), Frame, ref h.F);
                }
                h.Line = new ColourVector();
                h.Line = physicalLevel.NextColourOnLine(h.pos, Clamper.clamp(h.Outputs[(int)BleepyBloop.OAL.LineAngleOut] * 0.5) - 0.5+h.Rotation,Frame);
                for (int j = 0; j < 4; j++)
                {
                    h.Inputs[j * 3] = h.L.CoOrdinates[j];
                    h.Inputs[j * 3 + 1] = h.R.CoOrdinates[j];
                    h.Inputs[j * 3 + 2] = h.F.CoOrdinates[j];
                    h.Inputs[j + 20] = h.Line.CoOrdinates[j];
                }
           }
        }

        public void SimulateBrain(int Frame)
        {
            bleepSim.Bloops = Bloops;
            bleepSim.RunThreads();
            Bloops = bleepSim.Bloops;
        }

        public void SimulatePhy(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                ColideCollisions(Frame, i);
                EatAvailibleFood(Frame, i);
                RemovePoisonFromBloop(Frame, i);
                EatAvailiblePoison(Frame, i);
            }
            
            physicalLevel.NewFoodAndPoison();
        }

        public override bool Step()
        {
            //NullCheck(Frame);
            
                MoveFromPhyToBrain(Frame);
            
            //NullCheck(Frame);
            if(Bloops.Any(a => !a.GenesCompiled))
            {
                foreach (BleepyBloop B in Bloops)
                {
                    B.CompileGenes();
                }
            }
            SimulateBrain(Frame);
            //NullCheck(Frame);
            MoveFromBrainToPhy(Frame);
            //NullCheck(Frame);
            SimulatePhy(Frame);
            //NullCheck(Frame);
            //CheckBloopsAreInSameOrder();
            Frame++;
            if (Frame > NF)
            {
                for (int i = 0; i < bleepSim.Hs.Count(); i++)
                {
                    bleepSim.WaiterA[i].Set();
                    bleepSim.Hs[i].Join();
                }
            }

            return (Frame > NF);
        }

        private static Field InitNewFieldWithOldBleeps(BleepyBloop[] Olds)
        {
            Field F = new Field(Olds.Count());
            F.Bloops = new BleepyBloop[Olds.Count()];
            F.physicalLevel.Foods = new List<Food>();
            F.physicalLevel.Poisons = new List<Poison>();
            for (int i = 0; i < Olds.Count(); i++)
            {
                F.Bloops[i] = new BleepyBloop();
                F.Bloops[i] = new BleepyBloop();
                F.Bloops[i].Genes = Olds[i].Genes;
                F.Bloops[i].Food += Olds[i].Food;
            }

            return F;
        }

        private void ColideCollisions(int Frame, int i)
        {
            int BloopCount = Bloops.Count();
            for (int oo = 0; oo < BloopCount; oo++)
            {
                if (i < oo)
                {
                    if (Math.Abs(Bloops[i].Position.x - Bloops[oo].pos.x) < 2d)
                    {
                        if (Math.Abs(Bloops[i].Position.y - Bloops[oo].pos.y) < 2d)
                        {
                            Vector2d Diff = Bloops[oo].pos - Bloops[i].pos;
                            Bloops[i].pos -= 0.5d * Diff;
                            Bloops[oo].pos -= -0.5d * Diff;
                        }
                    }
                }
            }
            foreach (Rock r in physicalLevel.Rocks)
            {
                double diffX = (Bloops[i].Position.x - r.Position.x);
                if (diffX*diffX < 4d)
                {
                    double diffY = (Bloops[i].Position.y - r.Position.y);
                    if (diffY*diffY < 4d)
                    {
                        Vector2d Diff = Bloops[i].pos - r.Position;
                        Bloops[i].pos -= -1d * Diff;
                    }
                }
            }
        }

        private void EatAvailibleFood(int Frame, int i)
        {
            int FoodCount = physicalLevel.Foods.Count();
            for (int oo = 0; oo < FoodCount; oo++)
            {
                BleepyBloop localBloop = Bloops[i];
                Food food = physicalLevel.Foods[oo];
                double diffX = (localBloop.Position.x - food.Position.x);
                if (diffX*diffX < 2.25d)
                {
                    double diffY = (localBloop.Position.y - food.Position.y);
                    if (diffY*diffY < 2.25d)
                    {
                        if ((localBloop.Position - food.Position).absolute < 1.5)
                        {
                            localBloop.Food += food.Calories;
                            food.Calories = 0;
                        }
                    }
                }
            }
        }

        private void EatAvailiblePoison(int Frame, int i)
        {
            int LUP = physicalLevel.Poisons.Count();
            for (int oo = 0; oo < LUP; oo++)
            {
                BleepyBloop localBloop = Bloops[i];
                Poison poison = physicalLevel.Poisons[oo];
                double xDiff = (localBloop.Position.x - poison.Position.x);
                if (xDiff*xDiff < 2.25d)
                {
                    double yDiff = (localBloop.Position.y - poison.Position.y);
                    if (xDiff * xDiff < 2.25d)
                    {
                        if ((localBloop.Position - poison.Position).absolute < 1.5)
                        {
                            localBloop.Food = Math.Max(localBloop.Food - poison.PoisonSize, 0);
                            localBloop.Poison += poison.PoisonSize;
                            poison.PoisonSize = 0;
                        }
                    }
                }
            }
        }

        private void RemovePoisonFromBloop(int Frame, int i)
        {
            if (Bloops[i].Poison > 0)
            {
                Bloops[i].Poison -= 0.4;
            }
        }

        public void CheckBloopsAreInSameOrder()
        {
            if(PreviousOrderCheck.Count()!=Bloops.Count())
            {
                PreviousOrderCheck = new BleepyBloop[Bloops.Count()];
                for (int i = 0; i < Bloops.Count(); i++)
                {
                    PreviousOrderCheck[i] = (BleepyBloop)Bloops[i];
                }
            }
            for (int i = 0; i < Bloops.Count(); i++)
            {
                if (Bloops.OrderByDescending(y => y.Food).ToArray()[i].RandomID != PreviousOrderCheck.ToArray()[i].RandomID)
                {
                    Console.WriteLine(frame);
                }
            }
            for (int i = 0; i < Bloops.Count(); i++)
            {
                PreviousOrderCheck[i] = (BleepyBloop)Bloops[i];
            }
            PreviousOrderCheck = PreviousOrderCheck.OrderByDescending(y => y.Food).ToArray();
        }
    }
}