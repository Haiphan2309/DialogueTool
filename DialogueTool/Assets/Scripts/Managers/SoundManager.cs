using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Text;
using AudioPlayer;

namespace GDC.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        [SerializeField] GameObject sfxPrefab;
        [SerializeField] SoundMapConfig soundMapConfig;
        [SerializeField] int numberOfDefaultSFX = 8;

        [Space(10)]
        [SerializeField] float sfxVolume;
        [SerializeField] float musicVolume;

        [Space(10)]
        [SerializeField] AudioMixer currentMixer;
        [SerializeField] AudioMixerGroup SFX_MixerGroup;
        [SerializeField] AudioMixerGroup Music_MixerGroup;

        private bool isAdPauseUserMusic = false;

        private List<AudioSourceController> sfxAudioSrcChannel;
        private Dictionary<SoundID, AudioSource> aloneAudioSourceChannel;
        private Dictionary<SoundID, IEnumerator> aloneAudioSourceFadeCommand;
        private Dictionary<SoundID, int> continuousTimeSampleDict;
        private List<SoundID> m_soundIDPlayedInCurrentFrame;

        private const string MASTER_VOLUME = "MasterVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SFXVolume";
        private const string AMBIENCE_VOLUME = "AmbienceVolume";

        private bool isDisableMusic = false;

        private Queue<string> _logQueue;
        private StringBuilder _logStringBuilder;

        private List<SoundMap> soundMaps;
        public static System.Action ON_FINISH_LOADING_SOUNDMAP;

        private AudioSource currentMusicAudioSource;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this._logQueue = new Queue<string>();
            this._logStringBuilder = new StringBuilder();

            sfxAudioSrcChannel = new List<AudioSourceController>();
            aloneAudioSourceChannel = new Dictionary<SoundID, AudioSource>();
            aloneAudioSourceFadeCommand = new Dictionary<SoundID, IEnumerator>();
            continuousTimeSampleDict = new Dictionary<SoundID, int>();
            m_soundIDPlayedInCurrentFrame = new List<SoundID>();
            this.enabled = false;

            this.soundMaps = new List<SoundMap>();
            LoadCommonSoundMaps();

            StartCoroutine(Init());
        }

        public IEnumerator Init()
        {
            for (int i = 0; i < numberOfDefaultSFX; i++)
            {
                AudioSourceController newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSourceController>();
                sfxAudioSrcChannel.Add(newSFX);
            }
            yield return null;
            this.enabled = true;
            LoadSave();
            ON_FINISH_LOADING_SOUNDMAP?.Invoke();
        }

        private void OnAdPauseUserMusic()
        {
            if (!isAdPauseUserMusic)
            {
                isAdPauseUserMusic = true;
                SetMute(isAdPauseUserMusic);
            }
        }

        private void OnAdResumeUserMusic()
        {
            if (isAdPauseUserMusic)
            {
                isAdPauseUserMusic = false;
                SetMute(isAdPauseUserMusic);
            }
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.M))
            {
                // * Mute key cheat
                Debug.Log("M key was pressed.");
                TriggerDisableMusic();
            }
#endif
        }

        void TriggerDisableMusic()
        {
            isDisableMusic = !isDisableMusic;
            SetMusicMute(isDisableMusic);
        }

        void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime;
            /*if (sfxAudioSrcChannel.Count > 8)
            {
                AudioSource audioSource = sfxAudioSrcChannel.Find(x => !x.isPlaying);

                if (audioSource != null)
                {
                    sfxAudioSrcChannel.Remove(audioSource);
                    Destroy(audioSource.gameObject);
                }
            }*/
            if (Time.frameCount % 10 == 0)
            {
                for (int i = 0; i < sfxAudioSrcChannel.Count; i++)
                {
                    AudioSourceController item = sfxAudioSrcChannel[i];
                    if (!item.audioSource.isPlaying && item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }

            m_soundIDPlayedInCurrentFrame.Clear();
        }

        private AudioSource GetFreeSFXChannel(bool requestLowPriority = false)
        {
            AudioSourceController freeAS = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);

            /*if (freeAS == null)
            {
                AudioSource newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
                sfxAudioSrcChannel.Add(newSFX);
                return newSFX;
            }*/
            if (freeAS == null)
            {
                if (requestLowPriority)
                {
                    freeAS = sfxAudioSrcChannel.Find(x => x.IsLowPriority);
                    if (freeAS == null)
                    {
                        return null;
                    }
                }
                else
                {
                    int index = Random.Range(0, sfxAudioSrcChannel.Count);
                    freeAS = sfxAudioSrcChannel[index];
                }
            }
            return freeAS.audioSource;
        }

        private AudioSource GetAloneAudioSource(SoundID soundID)
        {
            AudioSource audioSource;
            if (!aloneAudioSourceChannel.TryGetValue(soundID, out audioSource))
            {
                AudioSourceController controller = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);

                if (controller == null)
                    audioSource = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
                else
                {
                    sfxAudioSrcChannel.Remove(controller);
                    audioSource = controller.audioSource;
                }
                aloneAudioSourceChannel.Add(soundID, audioSource);
            }
            return audioSource;
        }
        private AudioSource GetNewSFXChannel()
        {
            return Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
        }
        public AudioSource PlaySound(SoundClipData sfxData)
        {
            return PlaySound(sfxData, 1.0f);
        }
        string _logSoundClipDataFormat = "Frame <color=#FF00D9>{0}</color> Play SoundClipData <color=#FF00D9>{1} - {2}</color>";
        public AudioSource PlaySound(SoundClipData sfxData, float volume)
        {
            AudioSource sfxAudioSrc;

            sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);
            if (sfxAudioSrc == null)
                return null;

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = sfxData.IsLoop;

            sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;

            if (sfxData.IsPitch)
                sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();

            this.Log(string.Format(this._logSoundClipDataFormat, Time.frameCount, sfxData.name, sfxAudioSrc.clip.name));

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
            {
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            }
            return sfxAudioSrc;
        }

        string _logAudioClipFormat = "Frame <color=#00FFD2>{0}</color> Play AudioClip <color=#00FFD2>{1}</color>";
        public AudioSource PlaySound(AudioClip audioClip, float volume = 1f)
        {
            AudioSource sfxAudioSrc;

            sfxAudioSrc = GetFreeSFXChannel();
            if (sfxAudioSrc == null)
                return null;

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = false;

            sfxAudioSrc.outputAudioMixerGroup = this.SFX_MixerGroup;

            sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = audioClip;
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();

            this.Log(string.Format(this._logAudioClipFormat, Time.frameCount, Time.frameCount, sfxAudioSrc.clip.name));
            return sfxAudioSrc;
        }

        string _logSoundIDFormat = "Frame <color=#00FF30>{0}</color> Play SoundID: <color=#00FF30>{1}</color> - <color=#00FF30>{2}</color>";
        public AudioSource PlaySound(SoundID soundID, float volume = 1f)
        {
            if (soundID == SoundID.NONE) return null;

            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }

                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                string errorLog = string.Format("SoundMgr {0} missing mapping !", soundID.ToString());
                Debug.LogError(errorLog);
                this.Log(errorLog);
                return null;
            }

            for (int i = 0; i < m_soundIDPlayedInCurrentFrame.Count; i++)
            {
                if (m_soundIDPlayedInCurrentFrame[i] == soundID)
                {
                    string warningLog = string.Format("SoundMgr {0} already played in this frame!", soundID.ToString());
                    Debug.LogWarning(warningLog);
                    return null;
                }
            }
            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            AudioSource sfxAudioSrc;
            if (sfxData.IsAlone || sfxData.IsContinues)
            {
                sfxAudioSrc = GetAloneAudioSource(soundID);
            }
            else
                sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);

            if (sfxAudioSrc == null) return null;

            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return null;
            }

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = sfxData.IsLoop;

            sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;

            if (sfxData.IsPitch)
                sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();
            m_soundIDPlayedInCurrentFrame.Add(soundID);

            this.Log(string.Format(this._logSoundIDFormat, Time.frameCount, soundID, sfxAudioSrc.clip.name));

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            return sfxAudioSrc;
        }

        public AudioSource PlayMusic(SoundID soundID)
        {
            if (currentMusicAudioSource)
            {
                currentMusicAudioSource.Stop();
            }
            currentMusicAudioSource = this.PlaySound(soundID);
            currentMusicAudioSource.outputAudioMixerGroup = this.Music_MixerGroup;

            return currentMusicAudioSource;
        }
        public AudioSource PlayMusic(SoundID soundID, float volume = 1f)
        {
            if (currentMusicAudioSource)
            {
                currentMusicAudioSource.Stop();
            }
            currentMusicAudioSource = this.PlaySound(soundID, volume);
            currentMusicAudioSource.outputAudioMixerGroup = this.Music_MixerGroup;

            return currentMusicAudioSource;
        }
        public AudioSource PlayMusic(AudioClip audioClip, float volume = 1f)
        {
            if (currentMusicAudioSource)
            {
                currentMusicAudioSource.Stop();
            }
            currentMusicAudioSource = this.PlaySound(audioClip, volume);
            currentMusicAudioSource.loop = true;
            currentMusicAudioSource.outputAudioMixerGroup = this.Music_MixerGroup;

            return currentMusicAudioSource;
        }

        public void UnPauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            if (sfxData.IsAlone)
            {
                AudioSource sfxAudioSrc;
                if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;

                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }
                    sfxAudioSrc.UnPause();

                    coroutine = Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null);
                    StartCoroutine(coroutine);
                }
                else
                    PlaySound(soundID);
            }
            else if (sfxData.IsContinues)
            {
                AudioSource sfxAudioSrc;
                if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;
                }
                sfxAudioSrc = PlaySound(soundID);
                sfxAudioSrc.timeSamples = continuousTimeSampleDict[soundID];
            }
        }

        IEnumerator Coroutine_FadeInSFX(AudioSource aSrc, float time, System.Action action)
        {
            float timeOut = 0f;

            while (timeOut < 1f)
            {
                timeOut += Time.unscaledDeltaTime;
                float volume = timeOut / time;
                if (aSrc == null) yield break;
                aSrc.volume = volume;
                yield return null;
            }

            if (aSrc == null) yield break;
            aSrc.volume = 1f;

            if (action != null) action();
        }

        IEnumerator Coroutine_FadeOutSFX(AudioSource aSrc, float time, System.Action action)
        {
            float timeOut = time;

            while (timeOut > 0f)
            {
                timeOut -= Time.unscaledDeltaTime;
                float volume = timeOut / time;
                aSrc.volume = volume;
                yield return null;
            }

            aSrc.volume = 0f;

            if (action != null) action();
        }

        public void PauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;
            AudioSource sfxAudioSrc;
            if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc) && sfxAudioSrc.isPlaying)
            {
                if (sfxData.IsAlone)
                {
                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }

                    coroutine = Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () => sfxAudioSrc.Pause());
                    aloneAudioSourceFadeCommand.Add(soundID, coroutine);
                    StartCoroutine(coroutine);
                }

                if (sfxData.IsContinues)
                {
                    continuousTimeSampleDict[soundID] = sfxAudioSrc.timeSamples;

                    aloneAudioSourceChannel.Remove(soundID);
                    sfxAudioSrcChannel.Add(sfxAudioSrc.GetComponent<AudioSourceController>());

                    StartCoroutine(Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () =>
                    {
                        sfxAudioSrc.Stop();
                    }));
                }
            }
        }

        void SetVolume(string name, float value)
        {
            float db = 20.0f * (value < 0.001f ? -4.0f : Mathf.Log10(value));
            currentMixer.SetFloat(name, db);
            PlayerPrefs.SetFloat(name, value);
        }

        void SetVolume(string name, float value, bool needSave = true)
        {
            float db = 20.0f * (value < 0.001f ? -4.0f : Mathf.Log10(value));
            currentMixer.SetFloat(name, db);
            if (needSave)
            {
                PlayerPrefs.SetFloat(name, value);
            }
        }

        public void SetSFXVolume(float volumeValue, bool needSave = true)
        {
            sfxVolume = volumeValue;
            SetVolume(SFX_VOLUME, volumeValue, needSave);
            SetVolume(AMBIENCE_VOLUME, volumeValue, needSave);
        }
        public float GetMusicVolume()
        {
            return this.musicVolume;
        }
        public float GetSFXVolume()
        {
            return this.sfxVolume;
        }
        public void SetMusicVolume(float volumeValue, bool needSave = true)
        {
            musicVolume = volumeValue;
            SetVolume(MUSIC_VOLUME, volumeValue, needSave);
        }

        public IEnumerator SetMusicVolumeBack(float delaytime)
        {
            yield return new WaitForSeconds(delaytime);
            SetVolume(MUSIC_VOLUME, 1.0f);
        }

        public void SetMute(bool isMute = true)
        {
            SetVolume(MASTER_VOLUME, isMute ? 0.0f : 1.0f);
        }

        public void SetSFXMute(bool isMute = true)
        {
            sfxVolume = isMute ? 0.0f : 1.0f;
            SetVolume(SFX_VOLUME, sfxVolume);
            SetVolume(AMBIENCE_VOLUME, sfxVolume);
            Save();
        }

        public void SetMusicMute(bool isMute = true)
        {
            musicVolume = isMute ? 0.0f : 1.0f;
            SetVolume(MUSIC_VOLUME, musicVolume);
            Save();
        }

        private void LoadSave()
        {
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1.0f);

            SetVolume(MUSIC_VOLUME, musicVolume);
            SetVolume(SFX_VOLUME, sfxVolume);
            SetVolume(AMBIENCE_VOLUME, sfxVolume);
        }
        void Save()
        {
            PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
            PlayerPrefs.Save();
        }

        public string LOG_TAG = "SoundMgr";
        void Log(string log)
        {
            if (this._logQueue.Count > 20)
                this._logQueue.Dequeue();

            this._logQueue.Enqueue(log);

            this._logStringBuilder.Clear();
            foreach (string item in this._logQueue)
            {
                this._logStringBuilder.AppendLine(item);
            }
        }

        public string GetDebugLog()
        {
            return this._logStringBuilder.ToString();
        }

        private void LoadCommonSoundMaps()
        {
            LoadSoundMap(SoundType.COMMON);
            LoadSoundMap(SoundType.MUSIC);
            // LoadSoundMap(SoundType.MUSIC);
            LoadSoundMap(SoundType.SOUND_EFFECT);
            // LoadSoundMap(SoundType.AR);
        }

        public void LoadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                    return; // Already loaded
            }

            for (int i = 0; i < soundMapConfig.MapPath.Count; i++)
            {
                if (soundMapConfig.MapPath[i].Type == soundType)
                {
                    SoundMap soundMap = Resources.Load<SoundMap>(soundMapConfig.MapPath[i].Path);
                    soundMap.LoadSoundData();
                    this.soundMaps.Add(soundMap);
                    return;
                }
            }
        }

        public void UnloadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                {
                    Resources.UnloadAsset(this.soundMaps[i]);
                    this.soundMaps.RemoveAt(i);
                    //Resources.UnloadUnusedAssets();
                    return;
                }
            }
        }

        public SoundClipData GetSoundClipData(SoundID soundID)
        {
            if (soundID == SoundID.NONE) return null;

            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        return sound.Data;
                    }
                }
            }

            return null;
        }
        public void StopAllMusicPlaying()
        {

        }
    }
}
