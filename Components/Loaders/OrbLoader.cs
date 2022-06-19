using Promethium.Patches.Orbs.CustomOrbs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Components.Loaders
{
    public class OrbLoader : MonoBehaviour
    {
        private OrbPool _allOrbs;

        public void Start()
        {
            StartCoroutine(LateStart());
        }

        // We are delaying to make sure that this gets after the orb pool is made
        private IEnumerator LateStart()
        {
            yield return new WaitForSeconds(1.0f);
            RegisterOrbs();
        }

        private void RegisterOrbs()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll<OrbPool>();

            if (objects.Length == 0)
            {
                Plugin.Log.LogWarning("Could not find orb pool to inject custom orbs");
                return;
            }

            _allOrbs = objects[0] as OrbPool;

            List<GameObject> orbs = new List<GameObject>(_allOrbs.AvailableOrbs);

            if(Oreb.GetInstance().Registered)
                orbs.Add(Oreb.GetInstance().GetPrefab(1));
            if(OrbofGreed.GetInstance().Registered)
                orbs.Add(OrbofGreed.GetInstance().GetPrefab(1));

            _allOrbs.AvailableOrbs = orbs.ToArray();

            stopWatch.Stop();
            Plugin.Log.LogInfo($"Orbs Registered! Took {stopWatch.ElapsedMilliseconds}ms");
        }

    }
}
