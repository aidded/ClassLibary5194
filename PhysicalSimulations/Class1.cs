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

    public class InTime<T>
    {
        public List<T> AtFrame
        {
            get;
            set;
        }

        public InTime()
        {
            AtFrame = new List<T>();
        }

        public static InTime<T> FromList(List<T> Poo)
        {
            InTime<T> temp = new InTime<T>();
            temp.AtFrame = Poo;
            return temp;
        }
    }

    public static class InTime
    {
        public static InTime<T> CreateInTimeInstance<T>(int Frame, T First)
        {
            InTime<T> tmp = new InTime<T>();
            tmp.AtFrame = new List<T>();
            for (int f = 0; f <= Frame; f++)
            {
                tmp.AtFrame.Add(First);
            }
            return tmp;
        }
    }

    public abstract class LifeForm
    {
        public abstract Instruction[] Genes { get; set; }

        public abstract double ObjectiveFunction();
    }
}
