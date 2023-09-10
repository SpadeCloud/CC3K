using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace cc3k.Attributes
{
    public class RaceNameAttribute : Attribute
    {
        public string RaceName { get; private set; }
        public RaceNameAttribute(string name)
        {
            RaceName = name;
        }
    }
}
