using HarmonyLib;
using I2.Loc;
using System;
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
            foreach(String[] term in Plugin.LocalizationTerms)
            {
                String key = term[0];
                if (key == "Keys") continue;
                String[] values = new string[term.Length - 1];
                for(int i = 1; i < term.Length; i++)
                {
                    String value = term[i];
                    if (value == "") value = null;
                    values[i - 1] = term[i];
                }
                LocalizationManager.Sources[0].AddTerm(key).Languages = values;
            }
        }
    }
}
