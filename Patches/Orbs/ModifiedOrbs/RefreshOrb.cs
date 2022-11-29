using Battle.Attacks;
using BepInEx.Configuration;
using ProLib.Orbs;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedRefreshOrb : ModifiedOrb
    {
        private static ModifiedRefreshOrb _instance;

        private static readonly string _name = OrbNames.Refreshorb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");

        private ModifiedRefreshOrb() : base(OrbNames.Refreshorb) { }
        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            int level = attack.Level;
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER))
            {
                if (level >= 2)
                {
                    AddToDescription(attack, "refresh_on_hold");
                }
            }
        }
        public static ModifiedRefreshOrb Register()
        {
            if (_instance == null)
                _instance = new ModifiedRefreshOrb();
            return _instance;
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            Attack attack = heldOrb.GetComponent<Attack>();
            if (attack != null && attack.Level > 1)
            {
                Peg.OnPegAudioRequest?.Invoke(Peg.PegType.RESET);
                battleController.ResetField(false);
            }
        }
    }
}
