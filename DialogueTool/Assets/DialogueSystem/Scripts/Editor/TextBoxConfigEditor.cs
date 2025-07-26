using UnityEditor;

namespace DialogueSystem.Config
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
        }
    }
}
