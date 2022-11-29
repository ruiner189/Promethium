using ProLib.Relics;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethium.Patches.Relics.CustomRelics
{
    public sealed class CapsuleRelic : CustomRelic
    {
        public override void HandlePegHit(RelicManager relicManager)
        {
            if (CustomRelicManager.AttemptUseRelic(this))
            {
                relicManager.AddRelic(relicManager.GetBossRelic(true));
                CustomRelicManager.RemoveRelic(this);
            }
        }
    }
}
