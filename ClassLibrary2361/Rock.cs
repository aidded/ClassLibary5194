using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicalSimulations;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public class Rock : IPhysical<Vector2d>
    {
        public ColourVector Colour
        {
            get
            {
                return new ColourVector(0, 0, 1, 0);
            }
        }

        public Vector2d Position
        {
            get;
            set;
        }

        public Rock(double x,double y)
        {
            Position = new Vector2d(x, y);
        }

        public Rock()
        {
            GenerateRandomPosition();
        }

        private void GenerateRandomPosition()
        {
            Position = Vector2d.GenerateRandomPosition();
        }

        public double Size
        {
            get
            {
                return 0.7;
            }
        }
    }
}
