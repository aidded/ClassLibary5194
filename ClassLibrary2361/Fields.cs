using PhysicalSimulations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using ThinkingClassLibary;

namespace ClassLibrary2361
{
    public class Field : Generational
    {
        public PhysicalLevel physicalLevel;
        public BleepSim bleepSim;

        public BleepyBloop[] Bloops;

        public const int NF = 600;

        public Field(int NumberOfBloops)
        {
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
                Bloops[i].L = new ColourVector();
                Bloops[i].R = new ColourVector();
                Bloops[i].F = new ColourVector();
                physicalLevel.CAP(h.PosSensor(1d, 0.8), Frame, ref Bloops[i].L);
                physicalLevel.CAP(h.PosSensor(-1d, 0.8), Frame, ref Bloops[i].R);
                physicalLevel.CAP(h.PosSensor(0, 1.6), Frame, ref Bloops[i].F);
                for (int j = 0; j < 4; j++)
                {
                    h.Inputs[j * 3] = Bloops[i].L.CoOrdinates[j];
                    h.Inputs[j * 3 + 1] = Bloops[i].R.CoOrdinates[j];
                    h.Inputs[j * 3 + 2] = Bloops[i].F.CoOrdinates[j];
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
            if ((Frame % 4) == 0)
            {
                MoveFromPhyToBrain(Frame);
            }
            //NullCheck(Frame);
            SimulateBrain(Frame);
            //NullCheck(Frame);
            MoveFromBrainToPhy(Frame);
            //NullCheck(Frame);
            SimulatePhy(Frame);
            //NullCheck(Frame);
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
                            localBloop.Food += food.Size;
                            food.Size = 0;
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
                            localBloop.Food = Math.Max(localBloop.Food - poison.Size, 0);
                            localBloop.Poison += poison.Size;
                            poison.Size = 0;
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
    }
}