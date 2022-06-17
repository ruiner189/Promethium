using Relics;
using System.Linq;
using UnityEngine;

namespace Promethium.Components
{
    public class AutoScaler : MonoBehaviour
    {
        public RelicManager RelicManager;
        public Vector3 StartScale;
        public Vector3 EndScale;

        public const float TargetShrink = 0.5f;
        public const float TargetEnlarge = 2f;

        public const float Duration = 2f;

        public bool IsShrinking = false;

        public void Awake()
        {
            RelicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();
        }

        public void Start()
        {
            StartScale = transform.localScale;
            EndScale = transform.localScale;
        }

        public void Update()
        {
            if (IsShrinking)
            {
                Vector3 targetScale = new Vector3(StartScale.x * TargetShrink, StartScale.y * TargetShrink);
                Vector3 step = new Vector3(((targetScale.x - EndScale.x) / Duration) * Time.deltaTime, ((targetScale.y - EndScale.y) / Duration) * Time.deltaTime);
                transform.localScale += step;
                if (transform.localScale.x <= targetScale.x)
                {
                    EndScale = transform.localScale;
                    IsShrinking = false;
                }
            }
            else
            {
                Vector3 targetScale = new Vector3(StartScale.x * TargetEnlarge, StartScale.y * TargetEnlarge);
                Vector3 step = new Vector3(((targetScale.x - EndScale.x) / Duration) * Time.deltaTime, ((targetScale.y - EndScale.y) / Duration) * Time.deltaTime);
                transform.localScale += step;
                if (transform.localScale.y >= targetScale.x)
                {
                    EndScale = transform.localScale;
                    IsShrinking = true;
                }
            }
        }
    }
}
