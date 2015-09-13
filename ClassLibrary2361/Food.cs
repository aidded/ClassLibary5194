using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicalSimulations;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public class Food : IPhysical<Vector2d>
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
                return (new ColourVector(1d, 0d, 0d, 0d));
            }
        }

        public Food()
        {
            Size = Math.Pow(10, 1.176 + BetterRandom.StdDev(0.2d)); //random normal(mean,stdDev^2)
            GenerateRandomPosition();
        }

        private void GenerateRandomPosition()
        {
            Position = Vector2d.GenerateRandomPosition();
        }
    }
}
