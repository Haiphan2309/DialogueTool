using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

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

            Button renameAllElementsButton = new Button(RenameAllElement)
            {
                text = "Rename All Elements"
            };

            toolbar.Add(textField);
            toolbar.Add(saveButton);
            toolbar.Add(renameAllElementsButton);

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

        private void RenameAllElement()
        {
            foreach (var child in rootVisualElement.Children())
            {
                if (child is DSGraphView graphView)
                {
                    graphView.RenameAllElement();
                    return;
                }
            }
            Debug.LogError("Can't find graph view to rename all element!");
        }
    }
}
