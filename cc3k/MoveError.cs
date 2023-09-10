using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k
{
    public enum MoveError
    {
        None,
        Wall,
        MonsterOccupied,
        InvalidPath,
        PotionOccupied
    }
}
