using Core.AudioManager;
using Core.IInitializeQueue;
using Core.Other;
using Core.ServiceLocatorSystem;
using UnityEngine;

namespace Content.Features.SoundPlayers
{
    public class MusicPlayer : InitializeableMonoComponent
    {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private bool playOnInit;

        private IAudioManager audioManager;
        
        public override void SetupPriority()
        {
            initPriority = ConstantsHolder.INIT_QUEUE_LASTINIT_COMPONENTS;
            base.SetupPriority();
        }

        public override void Initialize()
        {
            audioManager = ServiceLocator.Get<IAudioManager>();
            base.Initialize();
            if (playOnInit && audioManager != null)
            {
                PlayMusic();
            }
        }

        public void PlayMusic()
        {
            audioManager?.PlayMusic(musicClip, true);
        }
    }
}