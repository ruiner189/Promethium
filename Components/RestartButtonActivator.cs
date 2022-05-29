using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
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
                else
                {
                    //Plugin.Log.LogMessage("Could not find restart button.");
                }
            }
            else
            {
                _restartButton = null;
            }
        }
    }
}
