using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Language
{
    [HarmonyPatch(typeof(GameInit), "Start")]
    public static class CreateLanguageTerms
    {
        public static void Postfix()
        {
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_damage_multiplier").Languages[0] = "Multiplies damage based on current <color=\"purple\">Armor</color>. Current multiplier: <color=\"purple\">%md</color>";
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_max").Languages[0] = "Increases Maximum <color=\"purple\">Armor</color> by <color=\"purple\">%am</color>";
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_turn").Languages[0] = "Restores <color=\"purple\">%ar</color> <color=\"purple\">Armor</color> every reload";
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_discard_max").Languages[0] = "Restores <color=\"purple\">Armor</color> to max if discarded";
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_discard").Languages[0] = "Restores <color=\"purple\">Armor</color> by <color=\"purple\">%ad</color> if discarded";
            LocalizationManager.Sources[0].AddTerm("Orbs/armor_damage_discard_multiplier").Languages[0] = "Discard to transfer multiplier to the next orb. Takes away all <color=\"purple\">Armor</color> and damages you for <color=\"red\">%ac</color>";
        }
    }
}
