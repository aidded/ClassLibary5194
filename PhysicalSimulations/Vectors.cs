using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary2361;
using ThinkingClassLibary;

namespace PhysicalSimulations
{
    public class Vector
    {
        public virtual double[] CoOrdinates
        {
            get;
            set;
        }

        public double dimensions
        {
            get
            {
                return CoOrdinates.Count();
            }
        }


    }

    public class Vector2d : Vector
    {
        public double x;
        public double y;

        public Vector2d(double X, double Y)
        {
            x = X;
            y = Y;
        }

        public override double[] CoOrdinates
        {
            get
            {
                return new double[2] { x, y };
            }

            set
            {
                x = value[0];
                y = value[1];
            }
        }

        public double absolute
        {
            get
            {
                return (Math.Sqrt(x * x + y * y));
            }
        }

        public double inverseSquare
        {
            get
            {
                return (1d / (x * x + y * y));
            }
        }

        public double inverseQuartic
        {
            get
            {
                return (1d / (x * x * x * x + y * y * y * y));
            }
        }

        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return (new Vector2d(a.x - b.x, a.y - b.y));
        }

        public static Vector2d operator *(double a, Vector2d b)
        {
            return (new Vector2d(a * b.x, a * b.y));
        }

        public Vector2d()
        {
        }

        public static Vector2d GenerateRandomPosition()
        {
            double Theta = BetterRandom.NextDouble() * Math.PI * 2;
            double Radius = BetterRandom.StdDev(40)+40;
            return (new Vector2d(Math.Sin(Theta) * Radius + 50, Math.Cos(Theta) * Radius + 50));
        }
    }

}