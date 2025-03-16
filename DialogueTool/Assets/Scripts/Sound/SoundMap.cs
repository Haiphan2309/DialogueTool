using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using Extensions;

public enum SoundType
{
    NONE = -1,
    COMMON = 0,
    MUSIC,
    SOUND_EFFECT,
}

namespace AudioPlayer
{
    [CreateAssetMenu(menuName = "Sound Configs/Sound Map")]
    public class SoundMap : ScriptableObject
    {
        public SoundType soundType;
        [SerializeField] string path = "SoundData";
        string mapPath = "SoundMaps";
        [ReadOnly]
        public List<SoundMapping> SoundMappingList;


        [Button("Load All Sound Data From Resources")]
        public void LoadSoundData()
        {

            var gos = Resources.LoadAll(path);
            // if (gos == null || gos.Length == 0) return;
            SoundMappingList = new List<SoundMapping>();
            foreach (var go in gos)
            {
                SoundClipData data = (SoundClipData)go;
                SoundMappingList.Add(new SoundMapping(data.Id, data));
                SoundMappingList.Sort();
            }

            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif

            SoundMapConfig config = GetSoundMapConfig();
            if (config == null) return;

            string soundMapName = this.name;
            string[] splittedName = soundMapName.Split(".");
            if (splittedName.Length != 0)
                soundMapName = splittedName[0];
            string soundMapPath = $"/{soundMapName}";

            foreach (var (path, idx) in config.MapPath.WithIndex())
            {
                if (path.Type == this.soundType)
                {
                    config.MapPath[idx].Path = this.mapPath + soundMapPath;
                    Debug.Log("Load Sound map Successfully");
                    return;
                }
            }
            config.MapPath.Add(new SoundMapConfig.SoundMapPath() {
                Type = this.soundType,
                Path = this.mapPath + soundMapPath
            });
            Debug.Log("Soundmap doesn't exist => Created new one");
            Debug.Log("Load Sound map Successfully");
        }
        SoundMapConfig GetSoundMapConfig()
        {
            var gos = Resources.LoadAll("");
            if (gos == null || gos.Length == 0) return null;
            SoundMapConfig config = null;
            foreach (var go in gos)
            {
                if (go.GetType() == typeof(SoundMapConfig))
                {
                    config = (SoundMapConfig)go;
                    return config;
                }
            }
            return null;
        }

        [Button]
        public void ClearSoundData()
        {
            SoundMappingList.Clear();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }

        [Button]
        public void EditSoundDefine()
        {
            #if UNITY_EDITOR
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/Audios/SoundID.cs"));
            #endif
        }
    }


    [Serializable]
    public class SoundMapping : IComparable
    {
        [HideInInspector] public string name;
        [ReadOnly] public SoundID Id;
        [ReadOnly] public SoundClipData Data;

        public SoundMapping(SoundID id, SoundClipData data)
        {
            this.name = id.ToString();
            this.Id = id;
            this.Data = data;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;
            SoundMapping s = (SoundMapping)obj;

            if (this.Id < s.Id)
                return -1;
            else if (this.Id > s.Id)
                return 1;
            return 0;
        }
    }
}


