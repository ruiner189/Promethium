using Battle.Attacks;
using Battle.Pachinko;
using BepInEx.Configuration;
using HarmonyLib;
using ProLib.Orbs;
using Promethium.Components.Managers;
using Promethium.Patches.Orbs.Attacks;
using Relics;
using System;
using System.Collections;
using UnityEngine;
using static BattleController;

namespace Promethium.Patches.Orbs.CustomOrbs.Potions
{
    public abstract class Potion : CustomOrb
    {
        public ConfigEntry<bool> EnabledConfig { internal set; get; }
        public bool ShouldSkip;
        public static bool UsedPotion;

        public Potion(String name) : base(name)
        {
            ShouldSkip = true;
        }

        public override bool IsEnabled()
        {
            if (EnabledConfig == null)
            {
                EnabledConfig = Plugin.ConfigFile.Bind<bool>("Custom Orbs", GetName(), true, "Disable to remove from orb pool");
            }

            return EnabledConfig.Value;
        }

        public override void OnShotFired(BattleController battleController, GameObject orb, Attack attack)
        {
            if (attack is PotionAttack potion)
            {
                if (potion.Temporary)
                {
                    if (HoldManager.Instance.SlotAvailable())
                    {
                        HoldManager.Instance.AddPotion(battleController, orb);
                        battleController._deckManager.RemoveOrbFromBattleDeck(orb);
                        battleController._activePachinkoBall = null;
                        if (ShouldSkip)
                        {
                            battleController._skipPlayerTurnCount--;
                            UsedPotion = true;
                        }
                    }
                    else
                    {
                        battleController._skipPlayerTurnCount--;
                        UsedPotion = true;
                    }
                } 
                else
                {
                    if (ShouldSkip) UsedPotion = true;
                }
            }

            orb.SetActive(false);
            battleController.StartCoroutine(ConsumePotion(battleController));
        }

        private IEnumerator ConsumePotion(BattleController battleController)
        {
            yield return new WaitForSeconds(0.6f); // We have to add a delay, otherwise players spamming potions experience weird glitches.
            if (battleController._deckManager.shuffledDeck.Count > 0)
            {
                BattleController._battleState = BattleState.AWAITING_ENEMY_CLEANUP;
            }
            else
            {
                battleController._remainingPachinkoBalls--;
            }
        }

        public virtual void ShotWhileInSlot(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject potion) {

        }
        public virtual void SetupWhileInSlot(BattleController battleController, GameObject attackingOrb, GameObject potion) { }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.InitializeAttack))]
        [HarmonyPostfix]
        private static void PatchInitialize(BattleController __instance)
        {
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (GetCustomOrbByName(attack.locNameString) is Potion potion)
                {
                    potion.SetupWhileInSlot(__instance, __instance._activePachinkoBall, obj);
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.ShotFired))]
        [HarmonyPrefix]
        private static void PatchShotFired(BattleController __instance)
        {
            if (BattleController._battleState == BattleState.NAVIGATION) return;
            foreach (GameObject obj in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = obj.GetComponent<PotionAttack>();
                if (GetCustomOrbByName(attack.locNameString) is Potion potion && potion != null)
                {
                    potion.ShotWhileInSlot(__instance._relicManager, __instance, __instance.activePachinkoBall, obj);
                }
            }
        }

        [HarmonyPatch(typeof(BattleController), nameof(BattleController.DoEndOfTurnBattleCleanup))]
        [HarmonyPrefix]
        private static void PatchCleanup(BattleController __instance)
        {
            if (UsedPotion)
            {
                UsedPotion = false;
                return;
            }
            foreach(GameObject potion in HoldManager.Instance.GetPotions())
            {
                PotionAttack attack = potion.GetComponent<PotionAttack>();
                if(attack != null)
                {
                    attack.Duration--;
                    if (attack.Duration <= 0)
                        HoldManager.Instance.RemovePotion(potion);
                }
            }
            HoldManager.Instance.UpdatePotionInfo();
        }

        [HarmonyPatch(typeof(PersistentOrb), nameof(PersistentOrb.modifiedPersistLevel), MethodType.Getter)]
        [HarmonyPostfix]
        private static void PatchPersists(PersistentOrb __instance, ref int __result)
        {
            if (__instance.GetComponent<PotionAttack>() != null)
            {
                __result = 0;
            }                
        }
    }
}
