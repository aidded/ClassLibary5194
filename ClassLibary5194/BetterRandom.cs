using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkingClassLibary
{
    public static class BetterRandom
    {
        public static Random R;
        public static double NextDouble()
        {
            if(R ==null)
            {
                R = new Random();
            }
            return R.NextDouble();
        }

        public static double StdDev(double StdDev)
        {
            double u1 = BetterRandom.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = BetterRandom.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            if (double.IsNaN(randStdNormal)) Console.Beep();
            return randStdNormal * StdDev;
        }
    }
}
