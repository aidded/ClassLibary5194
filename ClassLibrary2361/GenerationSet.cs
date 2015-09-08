using System.Collections.Generic;
using System.Xml.Serialization;
namespace ClassLibrary2361
{
    [XmlInclude(typeof(FieldSet))]
    public class GenerationSet
    {
        public List<Generational> Generations;
        public static GenerationSet NewField(LifeForm[] Olds) { return null; }
        public static GenerationSet NewField(int Number) { return null; }
    }
}
