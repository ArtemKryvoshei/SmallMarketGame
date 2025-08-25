using Core.AudioManager;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.SoundPlayers
{
    public class UIButtonWithSound : MonoBehaviour
    {
        [SerializeField] private AudioClip clickSound;

        public void PlaySound()
        {
            var audio = ServiceLocator.Get<IAudioManager>();
            audio.PlaySfx(clickSound, isUi: true);
        }
    }
}