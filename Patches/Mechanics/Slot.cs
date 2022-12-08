using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Patches.Mechanics
{
    public class Slot
    {
        public GameObject Container;
        public GameObject Orb;
        public GameObject Details;
        public GameObject DeckObject;
        public int Persists = -1;


        public bool Available()
        {
            return Details == null;
        }

        public void Reset()
        {
            Orb = null;
            Details = null;
            DeckObject = null;
            Persists = -1;
        }
    }
}
