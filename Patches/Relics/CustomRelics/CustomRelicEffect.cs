using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics
{

    public enum CustomRelicEffect
    {
        NONE = 7588,
        HOLSTER,
        MINI,
        WUMBO,
        KILL_BUTTON,
        PLASMA_BALL,
        // Leaving room between each curse set, just in case there are more added in. This allows future compatibility
        CURSE_ONE_BALANCE = 8000,
        CURSE_ONE_ATTACK,
        CURSE_ONE_CRIT,
        CURSE_TWO_HEALTH = 8020,
        CURSE_TWO_ARMOR,
        CURSE_TWO_EQUIP,
        CURSE_THREE_BOMB = 8040,
        CURSE_THREE_ATTACK,
        CURSE_THREE_CRIT,
        CURSE_FOUR_HEALTH = 8060,
        CURSE_FOUR_ARMOR,
        CURSE_FOUR_EQUIP,
        CURSE_FIVE_A = 8080,
        CURSE_FIVE_B,
        CURSE_FIVE_C
    }
}
