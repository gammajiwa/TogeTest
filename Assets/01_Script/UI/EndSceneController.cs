using UnityEngine;
using UnityEngine.SceneManagement;
using Toge.Core;

namespace Toge.UI
{
    public class EndSceneController : MonoBehaviour
    {
        [SerializeField] private string _menuScene = "MainMenu";

        private void Start()
        {
            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.HideLoading();
        }

        public void ToMenu()
        {
            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.Load(_menuScene);
            else SceneManager.LoadScene(_menuScene);
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
