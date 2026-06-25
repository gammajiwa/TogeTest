using UnityEngine;

namespace Toge.Core
{
    public static class ProceduralAudio
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateBgm()
        {
            const float duration = 8f;
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            float[] chord = { 220f, 277.18f, 329.63f };

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float swell = 0.55f + 0.45f * Mathf.Sin(2f * Mathf.PI * t / duration * 2f);
                float sample = 0f;
                foreach (float f in chord) sample += Mathf.Sin(2f * Mathf.PI * f * t);
                data[i] = sample / chord.Length * 0.16f * swell;
            }

            var clip = AudioClip.Create("BGM", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public static AudioClip CreateBlip(float frequency, float duration, float decay)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float envelope = Mathf.Exp(-t * decay);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.45f;
            }

            var clip = AudioClip.Create("Blip", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
