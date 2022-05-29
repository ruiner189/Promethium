using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class KillOnCommand : MonoBehaviour
    {
        public static bool Kill = false;
        public void Update()
        {
            if (Kill)
            {
                typeof(PachinkoBall).GetMethod("StartDestroy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(gameObject.GetComponent<PachinkoBall>(), new object[] { });
            }
        }
    }
}
