using UnityEngine;

namespace Core.AudioManager
{
    public interface IAudioManager
    {
        void PlayMusic(AudioClip clip, bool loop = true);
        void StopMusic();
        void PlaySfx(AudioClip clip, bool isUi = false);
    }
}