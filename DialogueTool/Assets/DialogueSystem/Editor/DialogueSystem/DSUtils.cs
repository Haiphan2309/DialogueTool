using DialogueSystem;
using DialogueSystem.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.Data;
using UnityEditor;
using System;

namespace DialogueSystem.Windows
{
    public static class DSUtils
    {
        const string assetFolderName = "DialogueDatas";
        const string assetPath = "Assets/DialogueSystem/Resources/" + assetFolderName;
        public static void SaveGraph(DSData dsData, string fileName)
        {
            SODialogue soDialogue = Resources.Load<SODialogue>($"{assetFolderName}/{fileName}");

            if (soDialogue == null)
            {
                soDialogue = ScriptableObject.CreateInstance<SODialogue>();
                AssetDatabase.CreateAsset(soDialogue, assetPath);
                Debug.Log($"Created new SODialogue at {assetPath}");
            }
            else
            {
                Debug.Log($"Overwriting existing SODialogue at {assetPath}");
            }

            soDialogue.DSData = GetCopyDSData(dsData);

            EditorUtility.SetDirty(soDialogue);
            AssetDatabase.SaveAssets();

            Debug.Log($"Saved dialogue to {assetPath}");
        }

        public static DSData LoadGraph(string fileName)
        {
            SODialogue soDialogue = Resources.Load<SODialogue>($"{assetFolderName}/{fileName}");

            if (soDialogue == null)
            {
                Debug.LogError($"Can't find SODialogue with path: {assetPath}");
                return null;
            }

            return GetCopyDSData(soDialogue.DSData);
        }

        private static DSData GetCopyDSData(DSData dsData)
        {
            DSData copyDsData = new DSData();

            /* Deep copy non ref data */

            foreach (var groupData in dsData.GroupDatas)
            {
                copyDsData.GroupDatas.Add(GetCopyDSGroupData(groupData));
            }

            foreach(var nodeData in dsData.UngroupedNodeDatas)
            {
                copyDsData.UngroupedNodeDatas.Add(GetCopyDSNodeData(nodeData));
            }

            copyDsData.StartNodeData = copyDsData.UngroupedNodeDatas[0];

            return copyDsData;
        }

        public static DSGroupData GetCopyDSGroupData(DSGroupData dsGroupData)
        {
            DSGroupData copyGroupData = new DSGroupData();

            copyGroupData.Index = dsGroupData.Index;
            copyGroupData.Name = dsGroupData.Name;
            copyGroupData.Position = dsGroupData.Position;

            foreach (var nodeData in dsGroupData.NodeDatas)
            {
                DSNodeData dsNodeData = GetCopyDSNodeData(nodeData);

                copyGroupData.NodeDatas.Add(dsNodeData);
            }

            return copyGroupData;
        }

        public static DSNodeData GetCopyDSNodeData(DSNodeData dsNodeData)
        {
            DSNodeData copyNodeData = new DSNodeData();

            copyNodeData.Index = dsNodeData.Index;
            copyNodeData.Name = dsNodeData.Name;
            copyNodeData.Position = dsNodeData.Position;
            copyNodeData.Text = dsNodeData.Text;
            copyNodeData.TextBoxType = dsNodeData.TextBoxType;
            copyNodeData.TalkingEmotion = dsNodeData.TalkingEmotion;
            copyNodeData.IsHaveCallBack = dsNodeData.IsHaveCallBack;
            copyNodeData.NextNodeIndex = dsNodeData.NextNodeIndex;
            copyNodeData.GroupDataIndex = dsNodeData.GroupDataIndex;

            foreach (var choiceData in dsNodeData.ChoiceDatas)
            {
                DSChoiceData copyChoiceData = new DSChoiceData(choiceData.Text);
                copyChoiceData.NextNodeIndex = choiceData.NextNodeIndex;

                copyNodeData.ChoiceDatas.Add(copyChoiceData);
            }

            return copyNodeData;
        }
    }

    [Serializable]
    public class Wrapper<T>
    {
        [SerializeField] private string _type;
        public string Type
        {
            get => _type;
            set => _type = value;
        }

        [SerializeField] private T _data;
        public T Data
        {
            get => _data;
            set => _data = value;
        }

        public Wrapper(string type, T data)
        {
            _type = type;
            _data = data;
        }
    }
}
