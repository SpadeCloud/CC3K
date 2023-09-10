using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k.Attributes
{
    public class CommandAttribute : Attribute
    {
        public string[] Prefix { get; private set; }

        public CommandAttribute(params string[] prefix)
        {
            Prefix = prefix;
        }
    }

    public class DirectionCommandAttribute : CommandAttribute
    {
        public DirectionCommandAttribute()
            : base(GameBoard.Directions)
        {

        }
    }
}
