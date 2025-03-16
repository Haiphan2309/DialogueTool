using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TipData
{
    public string tip;
}
[Serializable]
[CreateAssetMenu(menuName = "Config/UI/TipConfig")]
public class TipConfig : ScriptableObject
{
    //public List<TipData> commonTipDatas;
    //public List<TipChapterData> tipChapterDatas;
    public List<TipData> tipDatas;
}
