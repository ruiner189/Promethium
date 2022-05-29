using HarmonyLib;
using Promethium.Components;
using Relics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Promethium.Extensions.UI
{
    public class KillButton : MonoBehaviour
    {
        private static GameObject _prefab;
        public BattleController battleController;
        public RelicManager relicManager;

        public int State => (int)typeof(BattleController).GetField("_battleState", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(battleController);

        public void Awake()
        {
            battleController = Resources.FindObjectsOfTypeAll<BattleController>().FirstOrDefault();
            relicManager = Resources.FindObjectsOfTypeAll<RelicManager>().FirstOrDefault();
        }

        public void OnButtonPress()
        {
            if (battleController != null)
            {
                if (State == 3)
                {
                    KillOnCommand.Kill = true;
                    this.gameObject.SetActive(false);
                }
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


