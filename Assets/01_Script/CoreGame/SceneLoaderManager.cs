using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toge.Core
{
    public class SceneLoaderManager : MonoBehaviour
    {
        public static SceneLoaderManager Instance { get; private set; }

        [SerializeField] private GameObject _loadingScreen;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        public void ShowLoading()
        {
            if (_loadingScreen != null) _loadingScreen.SetActive(true);
        }

        public void HideLoading()
        {
            if (_loadingScreen != null) _loadingScreen.SetActive(false);
        }

        public void Load(string sceneName) => StartCoroutine(LoadRoutine(sceneName));

        private IEnumerator LoadRoutine(string sceneName)
        {
            ShowLoading();
            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (op != null && !op.isDone) yield return null;
        }
    }
}
