using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Language
{
    public class LanguageLoader : MonoBehaviour
    {
        public void Start()
        {
            StartCoroutine(LateStart());
        }

        // We are delaying because the base terms gets updated from google sheets. If we don't, our terms get removed.
        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(2);
            RegisterTerms();
        }

        public static void RegisterTerms()
        {
            foreach (String[] term in Plugin.LocalizationTerms)
            {
                String key = term[0];
                if (key == "Keys") continue;
                String[] values = new string[term.Length - 1];
                for (int i = 1; i < term.Length; i++)
                {
                    values[i - 1] = term[i];
                }
                LocalizationManager.Sources[0].AddTerm(key).Languages = values;
            }
            Plugin.Log.LogMessage("Localization loaded!");
        }
    }


    // This is used just in case the localization was overwritten at any point.
    [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetTranslation))]
    public static class MissingTerms
    {
        private static bool _fallback = true;
        public static void Postfix(ref string __result, string Term, bool FixForRTL, int maxLineLengthForRTL, bool ignoreRTLnumbers, bool applyParameters, GameObject localParametersRoot, string overrideLanguage, bool allowLocalizedParameters)
        {
            if (__result == null && _fallback && Plugin.LocalizationKeys.Contains(Term))
            {
                _fallback = false;
                LanguageLoader.RegisterTerms();
                __result = LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
                _fallback = true;
                if (__result == null) Plugin.Log.LogMessage($"Unable to find the term {Term}");
            }
        }
    }
}
