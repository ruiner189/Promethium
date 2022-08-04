using ProLib.Attributes;
using ProLib.Loaders;
using Promethium.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Promethium.UI
{
    [SceneModifier]
    public class RadialBar : MonoBehaviour
    {
        private static GameObject _prefab;
        public static RadialBar GarbageBar;
        private Image _background;
        private Image _loadingBar;
        private Image _foreground;
        private float _fillPercent;
        public float FillPercent { 
            get {
                return _fillPercent;
            }
            set {
                _fillPercent = Mathf.Clamp(value, 0, 1);

                if(_loadingBar != null)
                    _loadingBar.fillAmount = _fillPercent;
            }
        }

        public static GameObject GetPrefab()
        {
            if (_prefab != null) return _prefab;
            GameObject gameObject = new GameObject("RadialBar");
            gameObject.AddComponent<RadialBar>();
            gameObject.transform.localScale = new Vector2(1, 1);

            GameObject background = new GameObject("Background");
            background.transform.SetParent(gameObject.transform);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = Plugin.Circle;
            backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            background.transform.localScale = new Vector2(0.9f, 0.9f);

            GameObject foreground = new GameObject("Foreground");
            foreground.transform.SetParent(gameObject.transform);
            Image foregroundImage = foreground.AddComponent<Image>();
            foregroundImage.sprite = Plugin.Circle;
            foregroundImage.color = Color.black;
            foreground.transform.localScale = new Vector2(0.8f, 0.8f);

            GameObject loadingBar = new GameObject("LoadingBar");
            loadingBar.transform.SetParent(gameObject.transform);
            Image barImage = loadingBar.AddComponent<Image>();
            barImage.sprite = Plugin.Circle;
            barImage.color = Color.white;
            barImage.type = Image.Type.Filled;
            barImage.fillMethod = Image.FillMethod.Radial360;
            barImage.fillAmount = 0.0f;
            loadingBar.transform.localScale = new Vector2(0.8f, 0.8f);

            gameObject.transform.SetParent(Plugin.PromethiumPrefabHolder.transform);
            gameObject.HideAndDontSave();
            _prefab = gameObject;

            return gameObject;
        }

        public void Start()
        {
           _background = transform.GetChild(0).GetComponent<Image>();
           _foreground = transform.GetChild(1).GetComponent<Image>();
           _loadingBar = transform.GetChild(2).GetComponent<Image>();
        }

        public void SetForegroundColor(Color color)
        {
            _foreground.color = color;
        }

        public void SetBarColor(Color color)
        {
            _loadingBar.color = color;
        }

        public void SetBackgroundColor(Color color)
        {
            _background.color = color;
        }


        public static void LateOnSceneLoaded(String scene, bool firstLoad)
        {
            if(scene == SceneLoader.Battle)
            {
                GameObject parent = GameObject.Find("OrbDetails");
                if(parent != null)
                {
                    GameObject bar = GameObject.Instantiate(GetPrefab(), parent.transform);
                    bar.transform.localScale = new Vector2(0.012f, 0.012f);
                    bar.transform.localPosition = new Vector2(1.2f, 0.9f);
                    GarbageBar = bar.GetComponent<RadialBar>();
                    bar.SetActive(false);
                }
            }
        }
    }
}
