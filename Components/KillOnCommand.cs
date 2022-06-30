using UnityEngine;

namespace Promethium.Components
{
    [RequireComponent(typeof(PachinkoBall))]
    public class KillOnCommand : MonoBehaviour
    {
        public static bool Kill = false;
        private PachinkoBall _pachinko;

        public void Awake()
        {
            _pachinko = gameObject.GetComponent<PachinkoBall>();
        }

        public void Update()
        {
            if (Kill)
            {
                _pachinko.StartDestroy();
            }
        }
    }
}
