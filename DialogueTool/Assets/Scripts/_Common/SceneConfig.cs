using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using GDC.Enums;
using GDC.Managers;

namespace GDC.Configuration
{
    [CreateAssetMenu(menuName = "Scriptable Object/Scene Config", fileName = "Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        public SceneData data;
        public string GetSceneNameByType(SceneType type)
        {
            if (!this.data.ContainsKey(type))
                return GameSceneManager.TranslateToSceneName(SceneType.UNKNOWN);
            return this.data[type];
        }
    }
    [System.Serializable]
    public class SceneData : SerializableDictionaryBase<SceneType, string> {}
}
