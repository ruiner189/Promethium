using Battle.Attacks.DamageModifiers;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Relics
{
    public class CustomRelic : Relic
    {
        private RelicPool _pool = RelicPool.RARE_SCENARIO;
        public static List<CustomRelic> AllCustomRelics = new List<CustomRelic>();

        public static CustomRelic GetCustomRelic(CustomRelicEffect effect)
        {
            return AllCustomRelics.Find(relic => relic.effect == (RelicEffect)effect);
        }
        public CustomRelic()
        {
            AllCustomRelics.Add(this);
        }

        public void SetPoolType(RelicPool pool)
        {
            _pool = pool;
        }

        public RelicPool GetPoolType()
        {
            return _pool;
        }
    }

	// This is very messy. However, because the original method uses Mathf.Max, we must add our code to it. This needs to be switched to a transpiler for compatability and future game updates.
    [HarmonyPatch(typeof(Attack), nameof(Attack.GetModifiedDamagePerPeg))]
    public static class AddAttackPerPeg
    {
        public static bool Prefix(Attack __instance, RelicManager ____relicManager, CruciballManager ____cruciballManager, PlayerStatusEffectController ____playerStatusEffectController, DeckManager ____deckManager, int critCount, ref float __result)
        {
            if (____relicManager == null) return false;
            bool isCrit = critCount > 0;

			__result = isCrit ? __instance.CritDamagePerPeg : __instance.DamagePerPeg;

			if (____relicManager.RelicEffectActive(RelicEffect.INCREASE_STRENGTH_SMALL))
			{
				__result += 1f;
			}
			if (____relicManager.RelicEffectActive(RelicEffect.CONFUSION_RELIC))
			{
				__result += (float)(isCrit ? 3 : 2);
			}
			if (____relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
			{
				__result += 2f;
			}
			if (____relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
			{
				__result -= 2f;
			}
			if (critCount == 0 && ____relicManager.RelicEffectActive(RelicEffect.NON_CRIT_BONUS_DMG))
			{
				__result += 1f;
			}
			if (__instance.IsStone)
			{
				if (____relicManager.RelicEffectActive(RelicEffect.BASIC_STONE_BONUS_DAMAGE))
				{
					__result += (float)((critCount > 0) ? 2 : 1);
				}
				if (____cruciballManager.WeakStones())
				{
					__result += (float)((critCount > 0) ? -1 : 0);
				}
			}
			
			if (____playerStatusEffectController != null)
			{
				__result += (float)____playerStatusEffectController.EffectStrength(StatusEffectType.DmgBuff);
			}
			if (__instance.gameObject != null)
			{
				AttackBaseDamageModifier[] components = __instance.GetComponents<AttackBaseDamageModifier>();
				if (components != null && components.Length != 0)
				{
					foreach (AttackBaseDamageModifier attackBaseDamageModifier in components)
					{
						__result += attackBaseDamageModifier.GetDamageMod(____deckManager, ____relicManager, critCount);
					}
				}
			}
			if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_BALANCE))
                __result += 1;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_ATTACK) && !isCrit)
                __result += 2;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_CRIT) && isCrit)
                __result += 2;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_ATTACK) && !isCrit)
                __result += 2;
            if (____relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_CRIT) && isCrit)
                __result += 2;

			__result = Mathf.Max(__result, 0);

            return false;
        }
    }

}
