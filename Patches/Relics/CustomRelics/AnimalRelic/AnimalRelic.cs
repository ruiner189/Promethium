using System.Collections.Generic;
using UnityEngine;
using ProLib.Relics;

namespace Promethium.Patches.Relics.CustomRelics.AnimalRelic
{
    public class AnimalRelic : CustomRelic
    {
        public static List<AnimalRelic> AllAnimalRelics = new List<AnimalRelic>();

        public GameObject PegPrefab;
        public GameObject OrbPrefab;
        public int Level = 1;

        public AnimalRelic() : base()
        {
            AllAnimalRelics.Add(this);
        }

    }
}
