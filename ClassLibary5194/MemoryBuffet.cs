using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkingClassLibary
{
    public class MemoryBuffet
    {
        int MemorySize;
        double[] Memory;

        public double Get(double i)
        {
            int n = (int)Math.Round(ThinkingClassLibary.Clamper.clamp(i, 0d) * (MemorySize - 1));

            return Memory[n];
        }

        public void Set(double i, double o)
        {
            int n = (int)Math.Round(ThinkingClassLibary.Clamper.clamp(i, 0d) * (MemorySize - 1));
            Memory[n] = o;
        }

        public MemoryBuffet(int Size)
        {
            MemorySize = Size;
            Memory = new double[Size];
        }

        public MemoryBuffet()
        {
        }

        public double Max()
        {
            return Memory.Max();
        }
    }
}
