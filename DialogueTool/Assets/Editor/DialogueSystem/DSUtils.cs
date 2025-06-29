using DialogueSystem;
using DialogueSystem.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.Data;
using UnityEditor;

namespace DialogueSystem.Windows
{
    public static class DSUtils
    {
        const string path = "Assets/DialogueSystem/ScriptableObjects/SODialogues";
        public static void SaveGraph(DSData dsData, string fileName)
        {
            SODialogue soDialogue = ScriptableObject.CreateInstance<SODialogue>();
            soDialogue.DSData = GetCopyDSData(dsData);

            if (!AssetDatabase.IsValidFolder(path))
            {
                Debug.LogError($"Path '{path}' is not a valid folder in Assets");
                return;
            }

            string assetPath = $"{path}/{fileName}.asset";
            AssetDatabase.CreateAsset(soDialogue, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Saved dialogue to {assetPath}");
        }

        public static DSData LoadGraph(string soDialogueFileName)
        {
            string assetPath = $"{path}/{soDialogueFileName}.asset";

            SODialogue soDialogue = AssetDatabase.LoadAssetAtPath<SODialogue>(assetPath);

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

            foreach(var nodeData in dsData.UngroupNodeDatas)
            {
                copyDsData.UngroupNodeDatas.Add(GetCopyDSNodeData(nodeData));
            }

            copyDsData.StartNodeData = copyDsData.UngroupNodeDatas[0];

            return copyDsData;
        }

        private static DSGroupData GetCopyDSGroupData(DSGroupData dsGroupData)
        {
            DSGroupData copyGroupData = new DSGroupData();

            copyGroupData.Index = dsGroupData.Index;
            copyGroupData.Name = dsGroupData.Name;
            copyGroupData.Position = dsGroupData.Position;

            foreach (var nodeData in dsGroupData.NodeDatas)
            {
                DSNodeData dsNodeData = GetCopyDSNodeData(nodeData);
                dsNodeData.GroupData = copyGroupData;

                copyGroupData.NodeDatas.Add(dsNodeData);
            }

            return copyGroupData;
        }

        private static DSNodeData GetCopyDSNodeData(DSNodeData dsNodeData)
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

            foreach (var choiceData in dsNodeData.ChoiceDatas)
            {
                DSChoiceData copyChoiceData = new DSChoiceData(choiceData.Text);
                copyChoiceData.NextNodeIndex = choiceData.NextNodeIndex;

                copyNodeData.ChoiceDatas.Add(copyChoiceData);
            }

            return copyNodeData;
        }
    }
}
