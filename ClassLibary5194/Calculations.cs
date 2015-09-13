using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ThinkingClassLibary
{
    [XmlInclude(typeof(AddOrMultiply))]
    [XmlInclude(typeof(EqualsOrNot))]
    [XmlInclude(typeof(Compare))]
    public abstract class TwoCalculation
    {
        public abstract double PropertyA { get; set; }
        public abstract double PropertyB { get; set; }
        public abstract double Evaluate(double A, double B);
        public string Name
        {
            get
            {
                return this.GetType().ToString();
            }
        }
    }

    public class AddOrMultiply: TwoCalculation
    {
        double AddMultiply;
        double DiscreteContinous;

        public override double PropertyA { get { return AddMultiply; } set { AddMultiply = value; } }
        public override double PropertyB { get { return DiscreteContinous; } set { DiscreteContinous = value; } }

        public override double Evaluate(double A, double B)
        {
            double C = new double();
            if(DiscreteContinous<=0)
            {
                if(AddMultiply<=0)
                {
                    C = A + B;
                }
                else
                {
                    C = A * B;
                }
            }
            else
            {
                C = (AddMultiply / 2d + 0.5d) * (A + B) + (AddMultiply / -2d + 0.5d) * (A * B);
            }
            return Clamper.clamp(C,-1024,1024);
        }
    }

    public class Compare : TwoCalculation
    {
        double OutputMagnitude;
        double DiscreteContinous;

        public override double PropertyA { get { return OutputMagnitude; } set { OutputMagnitude = value; } }
        public override double PropertyB { get { return DiscreteContinous; } set { DiscreteContinous = value; } }

        public override double Evaluate(double A, double B)
        {
            double C = new double();
            if (DiscreteContinous <= 0)
            {
                if(A>B)
                {
                    C = OutputMagnitude;
                }
                else if(B>A)
                {
                    C = -OutputMagnitude;
                }
                else
                {
                    C = 0;
                }
            }
            else
            {
                C = (A - B) / (DiscreteContinous);
            }
            return C;
        }
    }

    public class EqualsOrNot: TwoCalculation
    {
        double DiscreteContinous;
        double EqualsNot;

        public override double PropertyA { get { return EqualsNot; } set { EqualsNot = value; } }
        public override double PropertyB { get { return DiscreteContinous; } set { DiscreteContinous = value; } }

        public override double Evaluate(double A, double B)
        {
            if (DiscreteContinous<=0)
            {
                if (EqualsNot<=0)
                {
                    return A;
                }
                else
                {
                    if (A == 0)
                        return B;
                    else
                        return 0;
                }
            }
            else
            {
                if ((A > 0))
                {
                    return Math.Pow(A, -EqualsNot);
                }
                else if (A < 0)
                {
                    return (-Math.Pow(-A, -EqualsNot));
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public static class Calcuations
    {
        public static TwoCalculation GetBlankFromInt(int i)
        {
            switch (i)
            {
                case 0: return new AddOrMultiply();
                case 1: return new EqualsOrNot();
                case 2: return new Compare();
                default: return null;
            } 
        }

        public static TwoCalculation GetBlankFromDouble(double d)
        {
            return GetBlankFromInt((int)Math.Round(d,0));
        }

        public static TwoCalculation GetRandom()
        {
            TwoCalculation c = (GetBlankFromDouble((BetterRandom.NextDouble() * 3) - 0.5));
            c.PropertyA = (BetterRandom.NextDouble() - 0.5) * 2;
            c.PropertyB = (BetterRandom.NextDouble() - 0.5) * 2;
            return c;
        }
    }

    public static class Clamper
    {
        public static double clamp(double C)
        {
            if (C < -1d)
            {
                return -1;
            }
            else if (C > 1d)
            {
                return 1;
            }
            else
            {
                return C;
            }
        }

        public static double clamp(double C, double Min)
        {
            if (C < Min)
            {
                return Min;
            }
            else if (C > 1d)
            {
                return 1;
            }
            else
            {
                return C;
            }
        }

        public static double clamp(double C, double Min, double Max)
        {
            if (C < Min)
            {
                return Min;
            }
            else if (C > Max)
            {
                return Max;
            }
            else
            {
                return C;
            }
        }
    }
}
