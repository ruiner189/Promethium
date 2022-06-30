using Promethium.Patches.Orbs.CustomOrbs;
using Promethium.Patches.Relics;
using Relics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Promethium.SoftDependencies
{
    public static class CustomStartDeck
    {
        public static void ConvertPromethiumRelics(List<String> originalNames)
        {
            if (originalNames != null)
            {
                for(int i = 0; i < originalNames.Count; i++)
                {
                    try
                    {
                        CustomRelicEffect relicEffect = (CustomRelicEffect)Enum.Parse(typeof(CustomRelicEffect), originalNames[i]);
                        originalNames[i] = ((int)relicEffect).ToString();
                    }
                    catch (Exception){}
                }
            }
        }
    }
}
