using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace AudioPlayer
{
    [CreateAssetMenu(menuName = "Sound Configs/Sound Clip Data")]
    public class SoundClipData : ScriptableObject
    {
        public SoundID Id;

        //[ReorderableList]
        public List<AudioClip> Clips;

        [BoxGroup("Options")]
        public bool IsLoop = false;
        [BoxGroup("Options")]
        public bool IsPitch = false;
        [BoxGroup("Options")]
        public bool IsAlone = false;
        [BoxGroup("Options")]
        public bool IsContinues = false;
        [BoxGroup("Options")]
        public bool IsSequence = false;
        [BoxGroup("Options")]
        public bool IsLowPriority = false;
        [BoxGroup("Options")][MinValue(0f)]
        public float FadeInTime = 0f;
        [BoxGroup("Options")][MinValue(0f)]
        public float FadeOutTime = 0f;
        [BoxGroup("Options")]
        public AudioMixerGroup SpecificMixerGroup;

        private int currentSequenceIndex = 0;
        private bool isUseSeasonalClip = false;
        private List<AudioClip> seasonalClips;

#if UNITY_EDITOR
        [Button]
        void RenameToId()
        {
            UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath((Object)this), Id.ToString());
        }
#endif

        public AudioClip GetClip()
        {
            if (IsSequence)
            {
                currentSequenceIndex = (currentSequenceIndex + 1) % Clips.Count;
                return Clips[currentSequenceIndex];
            }

            if (Clips.Count > 0)
            {
                int index = Random.Range(0, Clips.Count);
                return Clips[index];
            }
            return Clips[0];
        }

        // public void Play()
        // {
        //     SoundManager.Instance?.PlaySound(this);
        // }
        // public void Play(float volume)
        // {
        //     SoundManager.Instance?.PlaySound(this, volume);
        // }

        public void SetSeasonalClip(List<AudioClip> value)
        {
            this.seasonalClips = value;
            this.isUseSeasonalClip = true;
        }
    }
}