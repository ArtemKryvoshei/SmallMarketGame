using UnityEngine;
using System.Collections.Generic;

namespace Core.AudioManager
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private List<AudioSource> sfxSources = new();

        private readonly Dictionary<AudioSource, float> _busyUntil = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            foreach (var src in sfxSources)
                _busyUntil[src] = 0f;
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySfx(AudioClip clip, bool isUi = false)
        {
            if (clip == null) return;

            var source = GetFreeSfxSource();
            if (source == null)
            {
                Debug.LogWarning("⚠ Нет свободных SFX источников!");
                return;
            }

            source.spatialBlend = isUi ? 0f : 1f; // 2D для UI, 3D для мира
            source.PlayOneShot(clip);

            _busyUntil[source] = Time.time + clip.length;
        }

        private AudioSource GetFreeSfxSource()
        {
            foreach (var kvp in _busyUntil)
            {
                if (Time.time >= kvp.Value)
                    return kvp.Key;
            }
            return null; // все заняты
        }
    }
}