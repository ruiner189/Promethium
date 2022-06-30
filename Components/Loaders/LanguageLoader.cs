using HarmonyLib;
using I2.Loc;
using Promethium.Components;
using Promethium.Patches.Relics;
using Promethium.Patches.Relics.CustomRelics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Promethium.Patches.Language
{
    public class LanguageLoader : MonoBehaviour, ILocalizationParamsManager
    {
        private bool _isRegistered = false;
        public static LanguageLoader Instance {  get; private set; }

        private static String CurrentRawLocalizationText;
        private static String PreviousRawLocalizationText;

        public static List<String[]> LocalizationTerms { get; private set; }
        public static List<String> LocalizationKeys { get; private set; }

        public static LanguageSourceData LanguageSource { get; private set; }

        public void Awake()
        {
            Instance = this;
            LanguageSource = new LanguageSourceData();
            LocalizationManager.AddSource(LanguageSource);
        }

        public void Start()
        {
            StartCoroutine(LoadLocalization());
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

        private IEnumerator LoadLocalization()
        {
            LocalizationTerms = Plugin.LocalizationTerms;
            LocalizationKeys = Plugin.LocalizationKeys;
            RegisterTerms(true);

            LoadLocalTranslationFile();
            if (PreviousRawLocalizationText != null)
            {
                LocalizationTerms = TranslateTSVFile(PreviousRawLocalizationText, out List<String> keys);
                LocalizationKeys = keys;
                RegisterTerms(false);
            }

            UnityWebRequest www = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/e/2PACX-1vRe82XVSt8LOUz3XewvAHT5eDDzAqXr5MV0lt3gwvfN_2n9Zxj613jllVPtdPdQweAap2yOSJSgwpPt/pub?gid=0&single=true&output=tsv");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Plugin.Log.LogWarning("Could not connect to google sheets. Using translation from local file.");
            }
            else
            {
                // Show results as text
                CurrentRawLocalizationText = www.downloadHandler.text;
            }
            www.Dispose();

            if((PreviousRawLocalizationText == null || PreviousRawLocalizationText != CurrentRawLocalizationText) && CurrentRawLocalizationText != null)
            {
                Plugin.Log.LogInfo("Localization updated from Google Sheets!");
                SaveTranslationFile(CurrentRawLocalizationText);
                LocalizationTerms = TranslateTSVFile(CurrentRawLocalizationText, out List<String> keys);
                LocalizationKeys = keys;
                RegisterTerms(true);
            } else if (PreviousRawLocalizationText != null && CurrentRawLocalizationText != null)
            {
                Plugin.Log.LogInfo("Language Source was up-to-date with Google Spreadsheet");
            } else
            {
                Plugin.Log.LogInfo("Could not connect to Google Sheets or find Local File. Using Internal file instead.");
            }
        }

        private String GetTranslationFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "Promethium_Translation.tsv");
        }

        public void LoadLocalTranslationFile()
        {
            String path = GetTranslationFilePath();
            if (File.Exists(path))
            {
                PreviousRawLocalizationText = File.ReadAllText(path);
            }
        }

        public void SaveTranslationFile(String localization)
        {
            String path = GetTranslationFilePath();
            File.WriteAllText(path, localization);
        }

        public List<String[]> TranslateTSVFile(String text, out List<String> terms)
        {
            List<String[]> results = new List<String[]>();
            terms = new List<String>();
            foreach(String line in text.Split('\n'))
            {
                String[] split = line.Split('\t');
                results.Add(split);
                if (split.Length > 1 && !terms.Contains(split[0]))
                {
                    terms.Add(split[0]);
                }
            }
            return results;
        }

        public void RegisterTerms(bool replace)
        {
            if(replace) LanguageSource.ClearAllData();
            foreach (String[] term in LocalizationTerms)
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

                LanguageSource.AddTerm(key).Languages = values;
            }
            _isRegistered = true;
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
            else if (Param == ParamKeys.GRAVITY_CHANGE_TIME)
                return $"{RealityMarble.GRAVITY_CHANGE_SECONDS}";
            else if (Param == ParamKeys.GRAVITY_REDUCTION)
                return $"{(1 - PocketMoon.GRAVITY_REDUCTION) * 100}%";
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
                LanguageLoader.Instance.RegisterTerms(false);
                __result = LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage, allowLocalizedParameters);
                _fallback = true;
                if (__result == null) Plugin.Log.LogWarning($"Unable to find the term {Term}");
            }
        }
    }
}
