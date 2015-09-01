using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicalSimulations;

namespace ClassLibrary2361
{

    public class ColourVector : Vector
    {
        double Green { get { return base.CoOrdinates[0]; } set { base.CoOrdinates[0] = value; } }
        double Orange { get { return base.CoOrdinates[1]; } set { base.CoOrdinates[1] = value; } }
        double Grey { get { return base.CoOrdinates[2]; } set { base.CoOrdinates[2] = value; } }
        double Blue { get { return base.CoOrdinates[3]; } set { base.CoOrdinates[3] = value; } }

        public ColourVector()
        {
            base.CoOrdinates = new double[4];
            for (int i = 0; i < 4; i++)
            {
                base.CoOrdinates[i] = 0;
            }
        }

        public ColourVector(double Green, double Orange, double Grey, double Blue)
        {
            base.CoOrdinates = new double[4];
            this.Green = Green;
            this.Orange = Orange;
            this.Grey = Grey;
            this.Blue = Blue;
        }

        public static ColourVector operator +(ColourVector a, ColourVector b)
        {
            ColourVector Q = new ColourVector();
            for (int i = 0; i < 4; i++)
            {
                Q.CoOrdinates[i] = a.CoOrdinates[i] + b.CoOrdinates[i];
            }
            return Q;
        }

        public void Add(ColourVector b)
        {
            Green += b.Green;
            Orange += b.Orange;
            Grey += b.Grey;
            Blue += b.Blue;
        }

        public void AddMultiply(ColourVector a, double b)
        {
            Green += a.Green * b;
            Orange += a.Orange * b;
            Grey += a.Grey * b;
            Blue += a.Blue * b;
        }

        public void Multiply(double b)
        {
            Green *= b;
            Orange *= b;
            Grey *= b;
            Blue *= b;
        }

    }
}
