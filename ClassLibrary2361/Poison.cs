using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicalSimulations;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public class Poison : IPhysical<Vector2d>
    {
        /// <summary>
        /// Log natural distributed - average of 15, +- 1 stdev -> 1.5 : 150
        /// </summary>
        public double Size;
        public Vector2d Position { get; set; }
        public ColourVector Colour
        {
            get
            {
                return (new ColourVector(0d, 0d, 0d, 1d));
            }
        }

        public Poison()
        {
            Size = Math.Pow(10, 1.176 + BetterRandom.StdDev(0.2d))/2; //random normal(mean,stdDev^2)
            GenerateRandomPosition();
        }

        private void GenerateRandomPosition()
        {
            double Theta = BetterRandom.NextDouble() * Math.PI * 2;
            double Radius = BetterRandom.StdDev(60);
            Position = new Vector2d(Math.Sin(Theta)*Radius+50, Math.Cos(Theta) * Radius+50);
        }
    }
}
