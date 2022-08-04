using Battle;
using BepInEx.Configuration;
using ProLib.Orbs;
using ProLib.Relics;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedShuffleOrb : ModifiedOrb
    {
        private static ModifiedShuffleOrb _instance;

        private static readonly string _name = OrbNames.ShuffleOrb;
        public static readonly ConfigEntry<bool> EnabledConfig = Plugin.ConfigFile.Bind<bool>("Orbs", _name, true, "Disable to remove modifications");

        private ModifiedShuffleOrb() : base(_name) { }
        public override bool IsEnabled()
        {
            return EnabledConfig.Value;
        }

        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            int level = attack.Level;
            if (CustomRelicManager.RelicActive(RelicNames.HOLSTER))
            {
                if (level == 2)
                {
                    ReplaceDescription(attack, "shuffle_on_hold", 1);
                }
                else if (level == 3)
                {
                    ReplaceDescription(attack, "shuffle_on_hold", 2);
                }
            }
        }
        public static ModifiedShuffleOrb Register()
        {
            if (_instance == null)
                _instance = new ModifiedShuffleOrb();
            return _instance;
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            PegManager pegManager = battleController._pegManager;
            Attack attack = heldOrb.GetComponent<Attack>();
            if (attack != null && attack.Level > 1)
            {
                pegManager.ShuffleSpecialPegs(true);
            }
        }
    }
}
