using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary2361;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public interface IPhysical<V>
    {
        V Position { get; set; }
        ColourVector Colour { get; }
    }

    public abstract class LifeForm
    {
        public abstract Instruction[] Genes { get; set; }

        public abstract double ObjectiveFunction();
    }
}
