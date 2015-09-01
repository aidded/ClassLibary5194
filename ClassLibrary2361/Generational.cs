using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2361
{
    public abstract class Generational
    {
        public abstract InTime<LifeForm>[] Lifes { get; }
        public int Frame { get; set; }
        public virtual bool Step() { return false; }
    }
}
