using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Attributes;

namespace cc3k.Entities
{
    public enum PlayerRace : int
    {
        [Stats(140,20,20), Description("")] //stats and desc as attributes
        Human,//race itself
        [Stats(100,20,30), Description("gold is doubled in value")]
        Dwarf,
        [Stats(140,30,10), Description("negative potions have positive effect")]
        Elf,
        [Stats(180,30,25), Description("gold is worth half value")]
        Orc,
    }
}
