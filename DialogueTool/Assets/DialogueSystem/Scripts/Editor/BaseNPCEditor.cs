using UnityEditor;
using UnityEngine;
using DialogueSystem.Data;

namespace DialogueSystem.UI
{
    [CustomEditor(typeof(BaseNPC), true)]
    public class BaseNPCEditor : Editor
    {
        private SerializedProperty _soDialogueProp;
        private SerializedProperty _talkingNPCDatasProp;

        private void OnEnable()
        {
            _soDialogueProp = serializedObject.FindProperty("_soDialogue");
            _talkingNPCDatasProp = serializedObject.FindProperty("_talkingNPCDatas");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            // Auto resize _talkingNPCDatas when _soDialogue is assigned
            SODialogue soDialogue = _soDialogueProp.objectReferenceValue as SODialogue;

            if (soDialogue != null && soDialogue.DSData != null)
            {
                int targetSize = soDialogue.DSData.GroupDatas.Count;

                if (_talkingNPCDatasProp.arraySize != targetSize)
                {
                    _talkingNPCDatasProp.arraySize = targetSize;
                }

                for (int i = 0; i < targetSize; i++)
                {
                    SerializedProperty element = _talkingNPCDatasProp.GetArrayElementAtIndex(i);

                    SerializedProperty nameProp = element.FindPropertyRelative("Name");
                    var groupData = soDialogue.DSData.GroupDatas[i];
                    if (groupData != null && !string.IsNullOrEmpty(groupData.Name))
                    {
                        nameProp.stringValue = groupData.Name;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
