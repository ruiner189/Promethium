using BepInEx.Configuration;
using HarmonyLib;
using Promethium.Extensions;
using Relics;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Promethium.Patches.Relics
{
    public class CustomRelic : Relic
    {
        private RelicPool _pool = RelicPool.RARE_SCENARIO;
        public static List<CustomRelic> AllCustomRelics = new List<CustomRelic>();
        private ConfigEntry<bool> _config;

        public static CustomRelic GetCustomRelic(CustomRelicEffect effect)
        {
            return AllCustomRelics.Find(relic => relic.effect == (RelicEffect)effect);
        }
        public CustomRelic()
        {
            AllCustomRelics.Add(this);
        }

        public void Register()
        {
            if (this is not CurseRelic && _config == null)
                _config = Plugin.ConfigFile.Bind<bool>("Custom Relics", locKey, true, "Disable to remove from relic pool. The relic itself is still in the game.");
        }

        public void SetPoolType(RelicPool pool)
        {
            _pool = pool;
        }

        public RelicPool GetPoolType()
        {
            return _pool;
        }

        public bool IsEnabled()
        {
            if (_config == null) return true;
            return _config.Value;
        }

        public virtual void OnRelicAdded(RelicManager relicManager)
        {

        }

        public virtual void OnRelicRemoved(RelicManager relicManager)
        {

        }

        public virtual void OnArmBallForShot(BattleController battleController)
        {

        }

        public static float GetDamageModifier(Attack attack, int critCount, float damage)
        {
            RelicManager relicManager = attack._relicManager;

            // Curse Relics
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_BALANCE))
                damage += 1;
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_ATTACK))
                damage += 2;
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_ATTACK))
                damage += 2;

            // Changes to vanilla relics

            if (ModifiedRelic.HasRelicEffect(RelicEffect.MATRYOSHKA) && relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
            {
                damage -= ModifiedRelic.MATRYOSHKA_SHELL_DAMAGE;
            }

            if (ModifiedRelic.HasRelicEffect(RelicEffect.NO_DISCARD) && relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
            {
                damage -= ModifiedRelic.NO_DISCARD_RELIC_DAMAGE;
            }

            return damage;
        }

        public static float GetCritModifier(Attack attack, int critCount, float damage)
        {
            RelicManager relicManager = attack._relicManager;
            
            // Curse Relics
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_BALANCE))
                damage += 1;
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_ONE_CRIT))
                damage += 2;
            if (relicManager.RelicEffectActive(CustomRelicEffect.CURSE_THREE_CRIT))
                damage += 2;

            // Changes to vanilla relics

            if (ModifiedRelic.HasRelicEffect(RelicEffect.MATRYOSHKA) && relicManager.RelicEffectActive(RelicEffect.MATRYOSHKA))
            {
                damage -= ModifiedRelic.MATRYOSHKA_SHELL_CRIT;
            }

            if (ModifiedRelic.HasRelicEffect(RelicEffect.NO_DISCARD) && relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
            {
                damage -= ModifiedRelic.NO_DISCARD_RELIC_CRIT;
            }
            return damage;
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
                if (code[i].opcode == OpCodes.Ldfld && code[i].operand == (object)AccessTools.Field(typeof(Attack), "_relicManager"))
                {
                    insertionIndex = i + 4;
                    break;
                }
            }

            #region Change Attack
            List<CodeInstruction> instructionsToInsert = new List<CodeInstruction>();

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load this (attack)

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_1)); // Load critCount

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_1)); // Load local variable num

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomRelic), nameof(CustomRelic.GetDamageModifier)))); // Call Method CustomRelic::GetDamageModifier
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_1)); // Set local variable num to the return of CustomRelic::DamageModifier
            #endregion

            #region Change Crit
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0)); // Load this (attack)

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_1)); // Load critCount

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_2)); // Load local variable num2

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomRelic), nameof(CustomRelic.GetCritModifier)))); // Call Method CustomRelic::CritModifier
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_2)); // Set local variable num2 to the return of CustomRelic::DamageModifier
            #endregion

            code.InsertRange(insertionIndex, instructionsToInsert);


            return code;
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.MaxDiscardedShots), MethodType.Getter)]
    public static class ChangeDiscards
    {
        public static void Postfix(RelicManager ____relicManager, ref int __result)
        {
            if (ModifiedRelic.HasRelicEffect(RelicEffect.NO_DISCARD) && ____relicManager.RelicEffectActive(RelicEffect.NO_DISCARD))
            {
                ModifiedRelic.NO_DISCARD_RELIC_REMOVED_DISCARDS = __result + 1;
                __result = 0;
            }
            else if (____relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
            {
                ModifiedRelic.NO_DISCARD_RELIC_REMOVED_DISCARDS = __result + 1;
                __result = 0;
            }
        }
    }

    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.AddRelic))]
    public static class RelicAdded
    {
        public static void Postfix(RelicManager __instance, Relic relic)
        {
            if(relic is CustomRelic customRelic)
            {
                customRelic.OnRelicAdded(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.RemoveRelic))]
    public static class RelicRemoved
    {
        public static void Prefix(RelicManager __instance, RelicEffect re)
        {
            if (__instance._ownedRelics.ContainsKey(re))
            {
                CustomRelic relic = CustomRelic.GetCustomRelic((CustomRelicEffect)re);
                if (relic != null)
                {
                    relic.OnRelicRemoved(__instance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RelicManager), nameof(RelicManager.Reset))]
    public static class AllRelicsRemoved
    {
        public static void Prefix(RelicManager __instance)
        {
            if (__instance._ownedRelics == null) return;
            foreach(Relic relic in __instance._ownedRelics.Values)
            {
                if(relic is CustomRelic customRelic)
                {
                    customRelic.OnRelicRemoved(__instance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleController), nameof(BattleController.ArmBallForShot))]
    public static class ArmBallForShot
    {
        [HarmonyPriority(Priority.Low)]
        public static void Prefix(BattleController __instance)
        {
            foreach (Relic relic in __instance._relicManager._ownedRelics.Values)
            {
                if (relic is CustomRelic customRelic)
                {
                    customRelic.OnArmBallForShot(__instance);
                }
            }
        }
    }
}
