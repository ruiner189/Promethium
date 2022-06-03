using I2.Loc;
using Promethium.Components;
using Promethium.Extensions;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Orbs.ModifiedOrbs
{
    public sealed class ModifiedLightningBall : ModifiedOrb
    {
        private static ModifiedLightningBall _instance;
        private static LineRenderer _line;
        private ModifiedLightningBall() : base(OrbNames.LightningBall) {}


        public override void ChangeDescription(Attack attack, RelicManager relicManager)
        {
            if (relicManager != null && relicManager.RelicEffectActive(CustomRelicEffect.HOLSTER))
                AddToDescription(attack, "lightning_on_hold");
        }

        public override void ShotWhileInHolster(RelicManager relicManager, BattleController battleController, GameObject attackingOrb, GameObject heldOrb)
        {
            PachinkoBall pachinko = attackingOrb.GetComponent<PachinkoBall>();
            ThunderOrbPachinko thunder = attackingOrb.GetComponent<ThunderOrbPachinko>();
            if (thunder == null)
            {
                thunder = attackingOrb.AddComponent<ThunderOrbPachinko>();
                thunder.numZaps = 1;

                if (_line == null)
                {
                    GameObject gameObject = Resources.Load<GameObject>("Prefabs/Orbs/LightningBall-Lvl3");
                    if (gameObject != null)
                    {
                        if (_line == null)
                        {
                            ThunderOrbPachinko component = gameObject.GetComponent<ThunderOrbPachinko>();
                            if (component != null)
                            {
                                _line = component._line;
                            }
                        }

                    }
                }

                GameObject line = GameObject.Instantiate<GameObject>(_line.gameObject, attackingOrb.transform);
                thunder._line = line.GetComponent<LineRenderer>();
            }
            else
            {
                thunder.numZaps++;
            }

            Plasma plasma = attackingOrb.GetComponent<Plasma>();
            if(plasma != null)
            {
                plasma.AddToDefault(1);
            }
        }

        public static ModifiedLightningBall Register()
        {
            if (_instance == null)
                _instance = new ModifiedLightningBall();
            return _instance;
        }
    }
}
