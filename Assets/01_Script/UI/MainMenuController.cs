using UnityEngine;
using UnityEngine.SceneManagement;
using Toge.Core;

namespace Toge.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string _gameScene = "Boot";

        private void Start()
        {
            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.HideLoading();
        }

        public void Play()
        {
            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.Load(_gameScene);
            else SceneManager.LoadScene(_gameScene);
        }

        public void OpenSettings()
        {
            if (SettingsManager.Instance != null) SettingsManager.Instance.OpenSettings();
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
