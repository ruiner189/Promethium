using UnityEngine;
using UnityEngine.SceneManagement;

namespace Promethium.Components
{

    public class RestartButtonActivator : MonoBehaviour
    {
        private GameObject _restartButton;

        public void Update()
        {
            if (SceneManager.GetActiveScene().name == "FinalWinScene")
            {
                if (_restartButton == null) _restartButton = GameObject.Find("ButtonPanel").transform.GetChild(0).gameObject;
                if (_restartButton != null && !_restartButton.gameObject.activeInHierarchy)
                {
                    Plugin.Log.LogMessage("Found restart button. Activating");
                    _restartButton.SetActive(true);
                }
            }
            else
            {
                _restartButton = null;
            }
        }
    }
}
