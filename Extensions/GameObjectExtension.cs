using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Extensions
{
    public static class GameObjectExtension
    {
        public static void HideAndDontSave(this GameObject gameObject, bool includeChildren = true)
        {
            if (includeChildren)
            {

                foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
            }

            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}
