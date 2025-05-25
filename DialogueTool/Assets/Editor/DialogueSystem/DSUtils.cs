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
            soDialogue.DSData = dsData;

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
            return new DSData();
        }
    }
}
