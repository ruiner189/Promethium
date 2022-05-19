using Battle.Attacks.DamageModifiers;
using Battle.StatusEffects;
using Cruciball;
using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

		public static float GetDamageModifier(RelicManager relicManager, CruciballManager cruciballManager, int critCount, float currentValue)
        {
			if(relicManager != null)
            {
				bool isCrit = critCount > 0;

				if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_BALANCE))
					currentValue += 1;
				if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_ATTACK) && !isCrit)
					currentValue += 2;
				if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_CRIT) && isCrit)
					currentValue += 2;
				if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_ATTACK) && !isCrit)
					currentValue += 2;
				if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_CRIT) && isCrit)
					currentValue += 2;
			}
			return currentValue;
		}
    }

	[HarmonyPatch(typeof(Attack), nameof(Attack.GetModifiedDamagePerPeg))]
	public class ChangeAttackPerPeg
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var code = new List<CodeInstruction>(instructions);
			int insertionIndex = 0;
			// Checking where the first relicManager is null. We can use that as an anchor for where we insert our code, as it is nearby.
			for (int i = 0; i < code.Count; i++)
			{
				if (code[i].opcode == OpCodes.Ldfld && code[i].operand == (object) AccessTools.Field(typeof(Attack), "_relicManager"))
				{
					insertionIndex = i + 4;
					break;
				}
			}

			List<CodeInstruction> instructionsToInsert = new List<CodeInstruction>();

			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load this (attack)
			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Attack), "_relicManager"))); // Loads _relicManager. Consumes this

			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load this (attack)
			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Attack), "_cruciballManager"))); // Loads _cruciballManager. Consumes this

			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_1)); // Load critCount


			instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_0)); // Load local variable num

			instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomRelic), nameof(CustomRelic.GetDamageModifier)))); // Call Method CustomRelic::GetDamageModifier
			instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_0)); // Set local variable num to the return of CustomRelic::DamageModifier

			code.InsertRange(insertionIndex, instructionsToInsert);

			return code;
		}
	}
}
