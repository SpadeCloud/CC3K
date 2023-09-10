using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k.Attributes
{
    public class TitleAttribute : Attribute
    {
        public string Title { get; private set; }

        public TitleAttribute(string name)
        {
            Title = name;
        }
    }
}
