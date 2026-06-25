using UnityEngine;
using UnityEngine.UI;

namespace Toge.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Toggle _fullscreenToggle;

        private float _master = 0.9f;
        private float _music = 0.7f;
        private float _sfx = 0.9f;
        private bool _fullscreen = true;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
            Load();
            BindUI();
        }

        private void Start() => Apply();

        private void Load()
        {
            _master = PlayerPrefs.GetFloat("vol_master", 0.9f);
            _music = PlayerPrefs.GetFloat("vol_music", 0.7f);
            _sfx = PlayerPrefs.GetFloat("vol_sfx", 0.9f);
            _fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        }

        private void BindUI()
        {
            if (_masterSlider != null) { _masterSlider.SetValueWithoutNotify(_master); _masterSlider.onValueChanged.AddListener(SetMaster); }
            if (_musicSlider != null) { _musicSlider.SetValueWithoutNotify(_music); _musicSlider.onValueChanged.AddListener(SetMusic); }
            if (_sfxSlider != null) { _sfxSlider.SetValueWithoutNotify(_sfx); _sfxSlider.onValueChanged.AddListener(SetSfx); }
            if (_fullscreenToggle != null) { _fullscreenToggle.SetIsOnWithoutNotify(_fullscreen); _fullscreenToggle.onValueChanged.AddListener(SetFullscreen); }
        }

        private void Apply()
        {
            AudioListener.volume = _master;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(_music);
                AudioManager.Instance.SetSfxVolume(_sfx);
            }
            Screen.fullScreen = _fullscreen;
        }

        public void SetMaster(float value) { _master = value; AudioListener.volume = value; PlayerPrefs.SetFloat("vol_master", value); }
        public void SetMusic(float value) { _music = value; if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(value); PlayerPrefs.SetFloat("vol_music", value); }
        public void SetSfx(float value) { _sfx = value; if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(value); PlayerPrefs.SetFloat("vol_sfx", value); }
        public void SetFullscreen(bool value) { _fullscreen = value; Screen.fullScreen = value; PlayerPrefs.SetInt("fullscreen", value ? 1 : 0); }

        public void OpenSettings() { if (_settingsPanel != null) _settingsPanel.SetActive(true); }
        public void CloseSettings() { if (_settingsPanel != null) _settingsPanel.SetActive(false); }
    }
}
