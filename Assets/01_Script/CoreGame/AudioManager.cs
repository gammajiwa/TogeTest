using UnityEngine;

namespace Toge.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource _music;
        private AudioSource _sfx;
        private AudioClip _attackSfx;
        private AudioClip _cardSfx;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;
            _music.clip = ProceduralAudio.CreateBgm();
            _music.Play();

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;

            _attackSfx = ProceduralAudio.CreateBlip(180f, 0.18f, 16f);
            _cardSfx = ProceduralAudio.CreateBlip(520f, 0.12f, 20f);
        }

        public void SetMusicVolume(float volume)
        {
            if (_music != null) _music.volume = volume;
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfx != null) _sfx.volume = volume;
        }

        public void PlayAttack() => Play(_attackSfx);
        public void PlayCard() => Play(_cardSfx);

        private void Play(AudioClip clip)
        {
            if (_sfx != null && clip != null) _sfx.PlayOneShot(clip);
        }
    }
}
