using Battle.Attacks;
using Battle.Enemies;
using Battle.Pachinko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Orbs.Attacks
{
    public class PotionAttack : Attack
    {

        public PotionBehavior Behavior;

        public bool Temporary;
        public int Duration;
        public bool AffectOtherPotions;

        public void Awake()
        {
            if (locDescStrings == null) locDescStrings = new string[0];
        }

        public void SetBehavior(PotionBehavior behavior)
        {
            Behavior = behavior;
        }

        public override void Fire(AttackManager attackManager, Enemy target, float[] dmgValues, float dmgMult, int bonus, int critCount = 0)
        {
            _attackManager.AttackAnimationEnded();
        }

        public override float GetDamage(AttackManager attackManager, float[] dmgValues, float dmgMult, int bonus, int critCount = 0, bool displayNegative = false)
        {
            return 0;
        }

        public override float GetModifiedDamagePerPeg(int critCount = 0)
        {
            return 0;
        }


    }
}
