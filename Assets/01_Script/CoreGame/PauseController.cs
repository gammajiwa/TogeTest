using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Toge.Core
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private string _menuScene = "MainMenu";

        private bool _paused;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;
            if (SceneManager.GetActiveScene().name == _menuScene) return;
            Toggle();
        }

        public void Toggle()
        {
            if (_paused) Resume();
            else Pause();
        }

        public void Pause()
        {
            _paused = true;
            Time.timeScale = 0f;
            if (_pauseMenu != null) _pauseMenu.SetActive(true);
        }

        public void Resume()
        {
            _paused = false;
            Time.timeScale = 1f;
            if (_pauseMenu != null) _pauseMenu.SetActive(false);
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            _paused = false;
            if (_pauseMenu != null) _pauseMenu.SetActive(false);

            if (SceneLoaderManager.Instance != null) SceneLoaderManager.Instance.Load(_menuScene);
            else SceneManager.LoadScene(_menuScene);
        }
    }
}
