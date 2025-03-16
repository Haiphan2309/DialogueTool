using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Managers;

namespace AudioPlayer
{
    public class MusicController : MonoSingleton<MusicController>
    {
        private AudioSource currentAudioSource;
        public void PlayMusic(SoundID musicID)
        {
            if (currentAudioSource)
                currentAudioSource.Stop();
            currentAudioSource = SoundManager.Instance.PlayMusic(musicID);
        }
        // public void PlayMusic(AudioClip audioClip, float volume = 1f)
        // {
        //     if (currentAudioSource)
        //         currentAudioSource.Stop();
        //     currentAudioSource = SoundManager.Instance.PlayMusic(audioClip, volume);
        // }
        public void PauseCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.Pause();
        }
        public void ResumeCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.UnPause();
        }

        public void StopCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.Stop();
            // currentAudioSource.Stop();
        }

        public bool isPlaying(){
            return currentAudioSource.isPlaying;
        }
    }
}