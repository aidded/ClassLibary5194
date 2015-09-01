using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkingClassLibary
{
    public class MemAdr
    {
        int t;
        public int T
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
                if (value== 0)
                {
                    bT = false;
                }
                else
                {
                    bT = true;
                }
            }
        }
        public bool bT;
        public enum TInts :int
        {
            Memory = 0,
            Input = 1,
            Output = 2
        }

        public int P;
    }
}
