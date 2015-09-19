using System.Collections.Generic;
using System.Linq;
using PhysicalSimulations;
using System.Xml.Serialization;
using ThinkingClassLibary;
using System;

namespace ClassLibrary2361
{
    public class PhysicalLevel
    {
        private const double PiOverFour = Math.PI / 4;
        public List<Food> Foods;
        public int FrameValid;
        public List<Poison> Poisons;
        public List<Rock> Rocks;

        [XmlIgnore]
        public List<IPhysical<Vector2d>> ThingsCache;
        [XmlIgnore]
        public IPhysical<Vector2d>[] ThingsCacheXAsc;
        [XmlIgnore]
        public IPhysical<Vector2d>[] ThingsCacheYAsc;

        public void CAP(Vector2d Point, int Frame, ref ColourVector ot)
        {
            int l = ThingsCache.Count();
            for (int i = 0; i < l; i++)
            {
                IPhysical<Vector2d> Current = ThingsCache[i];
                double diffX = Current.Position.x - Point.x;
                if (diffX * diffX < 1024)
                {
                    double diffY = Current.Position.y - Point.y;
                    if (diffY * diffY < 1024)
                    {
                        if (diffX != 0 && diffY != 0)
                        {
                            double d2 = Vector2d.DistSquared(Current.Position, Point);
                            ot.AddMultiply(Current.Colour, 5184 / (d2 * d2));
                        }
                    }
                }
            }
            for (int i = 0; i < ot.dimensions; i++)
            {
                ot.CoOrdinates[i] += BetterRandom.StdDev(0.01);
            }
        }

        public ColourVector NextColourOnLine(Vector2d Point, double Angle, int Frame)
        {
            Angle = CircleifyAngle(Angle);
            IPhysical<Vector2d>[] Arr;
            int Dir;
            int Comp;

            GetArrayCompAndDirFromAngle(Angle, out Arr, out Dir, out Comp);

            int i = FindInitialIterator(Arr, Dir);
            i = MoveIteratorToFirstRelevantObject(Point, Arr, Dir, Comp, i);

            ColourVector c = new ColourVector(0, 0, 0, 0);
            double closest = FindPointsOnLine(Point, Angle, Arr, Dir, Comp, ref i, ref c);
            c.Multiply(512 / closest);
            return c;
        }

        private static double FindPointsOnLine(Vector2d Point, double Angle, IPhysical<Vector2d>[] Arr, int Dir, int Comp, ref int i, ref ColourVector c)
        {
            double closest = 3600;
            while (Dir * (Dir * (Arr[i].Position.CoOrdinates[Comp])) < (Dir * (Point.CoOrdinates[Comp] - (80 * Dir))))
            {
                Vector2d diff = (Arr[i].Position - Point);
                double DistSquared = Vector2d.DistSquared(Arr[i].Position, Point);
                if (DistSquared != 0)
                {
                    double AngleFromPoin = Vector2d.AngleTowards(Arr[i].Position, Point);
                    double NormalisedDistPerpendicularToThing = Math.Tan(Angle - AngleFromPoin);
                    double q = DistSquared * NormalisedDistPerpendicularToThing * NormalisedDistPerpendicularToThing;
                    if (q < (Arr[i].Size * Arr[i].Size))
                    {
                        if (DistSquared < closest)
                        {
                            closest = DistSquared;
                            c = Arr[i].Colour;
                        }
                    }
                }
                i++;
            }

            return closest;
        }

        private static int MoveIteratorToFirstRelevantObject(Vector2d Point, IPhysical<Vector2d>[] Arr, int Dir, int Comp, int i)
        {
            bool co = true;
            while (co)
            {
                if (Dir * (Dir * (Arr[i].Position.CoOrdinates[Comp])) < (Dir * (Point.CoOrdinates[Comp] - (80 * Dir))))
                {
                    i += Dir;
                }
                else
                {
                    co = false;
                }
            }

            return i;
        }

        private static int FindInitialIterator(IPhysical<Vector2d>[] Arr, int Dir)
        {
            int i = 0;
            if (Dir == 1)
            {
                i = 0;
            }
            else if (Dir == -1)
            {
                i = Arr.Count() - 1;
            }

            return i;
        }

        private static double CircleifyAngle(double Angle)
        {
            if (Angle > Math.PI * 2)
            {
                Angle -= Math.PI * 2;
            }
            else if (Angle < 0)
            {
                Angle += Math.PI * 2;
            }

            return Angle;
        }

        private void GetArrayCompAndDirFromAngle(double Angle, out IPhysical<Vector2d>[] Arr, out int Dir, out int Comp)
        {
            if (Angle < PiOverFour)
            {
                SetArrToBeHorazontal(out Arr, out Comp);
                Dir = 1;
            }
            else if (Angle < PiOverFour * 3)
            {
                SetArrToBeVertical(out Arr, out Comp);
                Dir = 1;
            }
            else if (Angle < PiOverFour * 5)
            {
                SetArrToBeHorazontal(out Arr, out Comp);
                Dir = -1;
            }
            else if (Angle < PiOverFour * 7 )
            {
                SetArrToBeVertical(out Arr, out Comp);
                Dir = -1;
            }
            else
            {
                SetArrToBeHorazontal(out Arr, out Comp);
                Dir = 1;
            }
        }

        private void SetArrToBeHorazontal(out IPhysical<Vector2d>[] Arr, out int Comp)
        {
            Arr = ThingsCacheXAsc;
            Comp = 0;
        }

        private void SetArrToBeVertical(out IPhysical<Vector2d>[] Arr, out int Comp)
        {
            Arr = ThingsCacheXAsc;
            Comp = 0;
        }

        public void AddNewFoodAndPoison(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Foods.Add(new Food());
                Foods[i] = new Food();
            }
            for (int i = 0; i < n; i++)
            {
                Poisons.Add(new Poison());
                Poisons[i] = new Poison();
            }
        }

        public void UpdateThingChache(Field F)
        {
            List<IPhysical<Vector2d>> J = new List<IPhysical<Vector2d>>();
            J.AddRange(F.Bloops);
            foreach (Food B in Foods)
            {
                J.Add(B);
            }
            foreach (Poison B in Poisons)
            {
                J.Add(B);
            }
            foreach (Rock R in Rocks)
            {
                J.Add(R);
            }
            ThingsCache = J;
            ThingsCacheXAsc = J.OrderBy(t => t.Position.x).ToArray();
            ThingsCacheYAsc = J.OrderBy(t => t.Position.y).ToArray();
            FrameValid = F.Frame;
        }

        public List<IPhysical<Vector2d>> Things(Field F)
        {
            if (F.Frame != FrameValid)
            {
                UpdateThingChache(F);
            }
            return ThingsCache;
        }

        public IPhysical<Vector2d>[] ThingsXAsc(Field F)
        {
            if (F.Frame != FrameValid)
            {
                UpdateThingChache(F);
            }
            return ThingsCacheXAsc;
        }

        public IPhysical<Vector2d>[] ThingsYAsc(Field F)
        {
            if (F.Frame != FrameValid)
            {
                UpdateThingChache(F);
            }
            return ThingsCacheYAsc;
        }

        public void NewFoodAndPoison()
        {
            int PoisonVariable = BetterRandom.PoisonVariable(0.01);
            for (int i = 0; i < PoisonVariable; i++)
            {
                Food tmp = new Food();
                Foods.Add(new Food());
            }

            PoisonVariable = BetterRandom.PoisonVariable(0.015);
            for (int i = 0; i < PoisonVariable; i++)
            {
                Poison tmp = new Poison();
                Poisons.Add(new Poison());
            }
        }
    }
}
