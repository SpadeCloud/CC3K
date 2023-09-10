using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Attributes;

namespace cc3k.Entities.Monsters
{
    public enum MonsterRace : int
    {
        [Stats(50, 25, 25), Description("")]
        Vampire,

        [Stats(120, 30, 5), Description("")]
        Werewolf,

        [Stats(120, 25, 15), Description("")]
        Troll,

        [Stats(70, 5, 10), Description("")]
        Kobold,

        [Stats(30, 70, 5)]
        [Description("Only hostile after a merchant is attacked, race is permanently hostile after\n\tIf not hosile, PC can buy, identify and sell potions through merchant")]
        Merchant,

        [Stats(150, 20, 20)]
        [Description("Always guards a dragon treasure horde")]
        Dragon,

        [Stats(50, 35, 20)]
        [Description("Respawns elsewhere on map once defeated")]
        Phoenix,
    }
}
