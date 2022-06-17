using HarmonyLib;
using I2.Loc;
using Promethium.Components;
using Promethium.Patches.Relics;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Promethium.Patches.Language
{
    public class LanguageLoader : MonoBehaviour, ILocalizationParamsManager
    {
        private bool _isRegistered = false;
        public static LanguageLoader Instance {  get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            StartCoroutine(LateStart());
        }

        public void OnEnable()
        {
            if (!LocalizationManager.ParamManagers.Contains(this))
            {

                LocalizationManager.ParamManagers.Add(this);

                LocalizationManager.LocalizeAll(true);
            }
        }

        public void OnDisable()
        {
            LocalizationManager.ParamManagers.Remove(this);
        }

        // We are delaying because the base terms gets updated from google sheets. If we don't, our terms get removed.
        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(2);
            if(!_isRegistered)
                RegisterTerms();
        }

        public void RegisterTerms()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (String[] term in Plugin.LocalizationTerms)
            {
                String key = term[0];
                if (key == "Keys") continue;
                String[] values = new string[term.Length - 1];
                for (int i = 1; i < term.Length; i++)
                {
                    if (term[i] != "")
                        values[i - 1] = term[i];
                    else
                        values[i - 1] = null;
                }
                LocalizationManager.Sources[0].AddTerm(key).Languages = values;
            }
            _isRegistered = true;
            stopwatch.Stop();
            Plugin.Log.LogInfo($"Localization loaded! Took {stopwatch.ElapsedMilliseconds}ms");
        }

        public string GetParameterValue(string Param)
        {
            if (Param == ParamKeys.ADDITIONAL_LIGHTNING_ZAPS) 
                return $"{Plasma.AdditionalZaps}";
            else if (Param == ParamKeys.PLASMA_PEG_HITS) 
                return $"{Plasma.PegsToHit}";
            else if (Param == ParamKeys.MULTIBALL_RELIC_MULTIPLIER) 
                return $"<style=dmg_negative>{(1 - ModifiedRelic.MATRYOSHKA_SHELL_MULTIPLIER) * 100}%</style>";
            else if (Param == ParamKeys.NO_DISCARD_MULTIPLIER) 
                return $"<style=dmg_bonus>{ModifiedRelic.NO_DISCARD_RELIC_MULTIPLIER * 100}%</style>";
            else if (Param == ParamKeys.NO_DISCARD_MULTIPLIER_VALUE)
            {
                float multiplier = ModifiedRelic.CalculateNoDiscardMultiplier() - 1;
                if (multiplier == 0) return "";
                return $"<style=dmg_bonus>({multiplier * 100}%)</style>";
            }
            return null;
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
                LanguageLoader.Instance.RegisterTerms();
                __result = LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
                _fallback = true;
                if (__result == null) Plugin.Log.LogWarning($"Unable to find the term {Term}");
            }
        }
    }
}
