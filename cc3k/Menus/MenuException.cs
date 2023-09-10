using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k.Menus
{
    public class MenuException : Exception
    {
        public MenuException(string message)
            : base(message)
        {

        }
    }
}
