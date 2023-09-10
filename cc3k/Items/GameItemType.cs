using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k.Items
{
    public enum GameItemType : int
    {
        // Potions
        IncHealth,
        IncAttack,
        IncDefense,
        DecHealth,
        DecAttack,
        DecDefense,
        // Gold
        NormalGold,
        SmallHordeGold,
        MerchantHordeGold,
        DragonHordeGold,
        // Staircase
        Staircase
    }
}
