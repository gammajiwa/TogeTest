using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Toge.Data;
using Toge.Enums;
using Toge.Events;

namespace Toge.Core
{
    public class GameFlowManager : MonoBehaviour
    {
        [SerializeField] private string _overworldScene = "Overworld";
        [SerializeField] private string _battleScene = "Battle";
        [SerializeField] private string _uiScene = "UI";
        [SerializeField] private bool _loadUiScene;

        [SerializeField] private EncounterEventChannelSO _encounterChannel;
        [SerializeField] private BattleResultEventChannelSO _resultChannel;

        private void OnEnable()
        {
            if (_encounterChannel != null) _encounterChannel.OnEventRaised += EnterBattle;
            if (_resultChannel != null) _resultChannel.OnEventRaised += ExitBattle;
        }

        private void OnDisable()
        {
            if (_encounterChannel != null) _encounterChannel.OnEventRaised -= EnterBattle;
            if (_resultChannel != null) _resultChannel.OnEventRaised -= ExitBattle;
        }

        private IEnumerator Start()
        {
            yield return LoadAdditive(_overworldScene);
            yield return LoadAdditive(_battleScene);
            if (_loadUiScene) yield return LoadAdditive(_uiScene);

            Activate(_overworldScene);
        }

        private void EnterBattle(EncounterSO encounter) => Activate(_battleScene);

        private void ExitBattle(BattleResult result) => Activate(_overworldScene);

        private void Activate(string sceneName)
        {
            string other = sceneName == _battleScene ? _overworldScene : _battleScene;
            SetSceneActive(other, false);
            SetSceneActive(sceneName, true);

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded) SceneManager.SetActiveScene(scene);
        }

        private static IEnumerator LoadAdditive(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) yield break;
            if (SceneManager.GetSceneByName(sceneName).isLoaded) yield break;
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private static void SetSceneActive(string sceneName, bool active)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded) return;

            foreach (GameObject root in scene.GetRootGameObjects())
                root.SetActive(active);
        }
    }
}
