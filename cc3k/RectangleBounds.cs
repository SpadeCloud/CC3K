using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k
{
    public class RectangleBounds
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public RectangleBounds(int x, int y, int width, int height)
        {
            X = x;
            Y = y; 
            Width = width;
            Height = height;
        }

        public bool Contains(int x, int y)
        {
            if (x < this.X) return false;
            if (x > this.X + this.Width) return false;
            if (y < this.Y) return false;
            if (y > this.Y + this.Height) return false;

            return true;
        }
    }
}
