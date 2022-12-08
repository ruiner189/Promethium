using ProLib.Attributes;
using ProLib.Managers;
using ProLib.Relics;
using Promethium.Components;
using Promethium.Patches.Relics.CustomRelics;
using Relics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Promethium.UI
{
    [SceneModifier]
    public class KillButton : MonoBehaviour
    {
        private static GameObject _prefab;
        public static GameObject currentButton;

        public static void LateOnSceneLoaded(string sceneName, bool firstLoad)
        {
            if(sceneName == SceneInfoManager.Battle)
            {
                RelicManager relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();
                if(relicManager != null)
                {
                   if (CustomRelicManager.Instance.RelicActive(RelicNames.KILL_BUTTON))
                       currentButton = CreateButton(new Vector3(12, -4.5f, 0));
                }
            }
        }

        public void OnButtonPress()
        {
            
            if (BattleController._battleState == BattleController.BattleState.AWAITING_SHOT_COMPLETION)
            {
                KillOnCommand.Kill = true;
                this.gameObject.SetActive(false);
            }
            
        }

        private static void CreatePrefab()
        {
            if (_prefab == null)
            {
                GameObject frame = new GameObject("KillButton");
                KillButton kb = frame.AddComponent<KillButton>();
                frame.AddComponent<Image>().sprite = Plugin.KillButton;
                Button button = frame.AddComponent<Button>();
                frame.transform.localScale = new Vector3(0.015f, 0.015f);

                DontDestroyOnLoad(frame);
                _prefab = frame;
            }
        }

        public static GameObject CreateButton(Vector3 position)
        {
            CreatePrefab();
            GameObject button = GameObject.Instantiate(_prefab);
            GameObject canvas = new GameObject("RelicButtonCanvas");
            Canvas component = canvas.AddComponent<Canvas>();
            component.sortingLayerName = "SidescrollerUI";
            component.sortingOrder = 3;
            canvas.AddComponent<CanvasGroup>().tag = "BossHealthBar";
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            button.transform.SetParent(canvas.transform);
            button.transform.position = position;
            button.GetComponent<Button>().onClick.AddListener(button.GetComponent<KillButton>().OnButtonPress);
            return button;

        }
    }
}


