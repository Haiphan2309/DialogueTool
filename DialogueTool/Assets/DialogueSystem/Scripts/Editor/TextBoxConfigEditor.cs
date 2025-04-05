using UnityEditor;
using System.Collections.Generic;

namespace DialogueSystem
{
    [CustomEditor(typeof(TextBoxConfig))]
    public class TextBoxConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TextBoxConfig config = (TextBoxConfig)target;

            DrawDefaultInspector();

            if (config.MinSize.x > config.MaxSize.x || config.MinSize.y > config.MaxSize.y)
            {
                EditorGUILayout.HelpBox("MinSize can not be larger than MaxSize!", MessageType.Error);
            }

            if (config.MinSize.x < 0 || config.MinSize.y < 0 || config.MaxSize.x < 0 || config.MaxSize.y < 0)
            {
                EditorGUILayout.HelpBox("MinSize or MaxSize can not be negative!", MessageType.Error);
            }

            if (!CheckAscending(config.PreferHorizontalConfigs))
            {
                EditorGUILayout.HelpBox("preferHorizontalConfigs.MinTextLength need to be sort ascending!", MessageType.Warning);
            }
        }

        private bool CheckAscending(List<PreferHorizontalConfig> preferHorizontalConfigs)
        {
            for (int i = 1; i < preferHorizontalConfigs.Count; i++)
            {
                if (preferHorizontalConfigs[i].MinTextLength < preferHorizontalConfigs[i - 1].MinTextLength)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
