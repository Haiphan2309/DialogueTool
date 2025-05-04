using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DialogueSystem.Windows
{
    public class DSEditorWindow : EditorWindow
    {
        [MenuItem("Window/DialogueSystem/Dialouge Editor")]
        public static void Open()
        {
            GetWindow<DSEditorWindow>("Dialogue Editor");
        }

        public void CreateGUI()
        {
            AddGraphView();
            AddStyle();
            AddToolBar();
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            TextField textField = new TextField()
            {
                value = "DialogueFileName",
                label = "File name"
            };

            Button saveButton = new Button(Save)
            {
                text = "Save"
            };

            toolbar.Add(textField);
            toolbar.Add(saveButton);

            rootVisualElement.Add(toolbar);
        }

        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void AddStyle()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void Save()
        {
            //todo
        }
    }
}
