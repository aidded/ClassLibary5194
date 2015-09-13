using System.Collections.Generic;
using System.Linq;
using PhysicalSimulations;
using System.Xml.Serialization;
using ThinkingClassLibary;

namespace ClassLibrary2361
{
    public class PhysicalLevel
    {
        public List<Food> Foods;
        public int FrameValid;
        public List<Poison> Poisons;
        public List<Rock> Rocks;

        [XmlIgnore]
        public List<IPhysical<Vector2d>> ThingsCache;

        public void CAP(Vector2d Point, int Frame, ref ColourVector ot)
        {
            int l = ThingsCache.Count();
            for (int i = 0; i < l; i++)
            {
                IPhysical<Vector2d> Current = ThingsCache[i];
                double diffX = (Current.Position.x - Point.x);
                if (diffX * diffX < 1024)
                {
                    double diffY = (Current.Position.y - Point.y);
                    if (diffY * diffY < 1024)
                    {
                        if (diffX != 0 && diffY != 0)
                        {
                            ot.AddMultiply(Current.Colour, 5184 * (Current.Position - Point).inverseQuartic);
                        }
                    }
                }
            }
            for (int i = 0; i < ot.dimensions; i++)
            {
                ot.CoOrdinates[i] += BetterRandom.StdDev(0.01);
            }
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
            J.AddRange(F.Bloops.Select(R => R.AtFrame[F.Frame]));
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

        public void NewFoodAndPoison()
        {
            int PoisonVariable = BetterRandom.PoisonVariable(0.01);
            for (int i = 0; i < PoisonVariable; i++)
            {
                InTime<Food> tmp = new InTime<Food>();
                Foods.Add(new Food());
            }

            PoisonVariable = BetterRandom.PoisonVariable(0.015);
            for (int i = 0; i < PoisonVariable; i++)
            {
                InTime<Poison> tmp = new InTime<Poison>();
                Poisons.Add(new Poison());
            }
        }
    }
}
