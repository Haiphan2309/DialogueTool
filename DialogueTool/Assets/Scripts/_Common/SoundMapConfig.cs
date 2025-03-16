using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AudioPlayer
{
    [CreateAssetMenu(menuName = "Sound Configs/Sound Map Config")]
    public class SoundMapConfig : ScriptableObject
    {
        //[ReorderableList]
        public List<SoundMapPath> MapPath;

        [System.Serializable]
        public class SoundMapPath
        {
            public SoundType Type;
            public string Path;
        }
    }
}
