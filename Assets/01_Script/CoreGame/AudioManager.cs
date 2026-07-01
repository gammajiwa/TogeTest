using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toge.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        [SerializeField] private AudioClip _menuBgm;
        [SerializeField] private AudioClip _gameplayBgm;

        [Header("SFX")]
        [SerializeField] private AudioClip _cardSfx;
        [SerializeField] private AudioClip _attackSfx;
        [SerializeField] private AudioClip _hitSfx;
        [SerializeField] private AudioClip _winSfx;
        [SerializeField] private AudioClip _loseSfx;

        private AudioSource _music;
        private AudioSource _sfx;
        private float _musicVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;

            SceneManager.sceneLoaded += OnSceneLoaded;
            ApplySceneBgm(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplySceneBgm(scene.name);

        private void ApplySceneBgm(string sceneName)
        {
            AudioClip target =
                sceneName == "MainMenu" || sceneName == "TheEnd" ? _menuBgm :
                sceneName == "Boot" ? _gameplayBgm :
                null;

            if (target == null || (_music.clip == target && _music.isPlaying)) return;

            _music.clip = target;
            _music.volume = _musicVolume;
            _music.Play();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            if (_music != null) _music.volume = volume;
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfx != null) _sfx.volume = volume;
        }

        public void PlayCard() => Play(_cardSfx);
        public void PlayAttack() => Play(_attackSfx);
        public void PlayHit() => Play(_hitSfx);
        public void PlayWin() => Play(_winSfx);
        public void PlayLose() => Play(_loseSfx);

        private void Play(AudioClip clip)
        {
            if (_sfx != null && clip != null) _sfx.PlayOneShot(clip);
        }
    }
}
