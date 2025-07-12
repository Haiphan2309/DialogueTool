using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using DialogueSystem.Data;

namespace DialogueSystem.Windows
{
    public class DSEditorWindow : EditorWindow
    {
        private DSGraphView m_dsGraphView;
        private TextField m_fileNameTextField;

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

            m_fileNameTextField = new TextField()
            {
                value = "DialogueFileName",
                label = "File name"
            };

            Button saveButton = new Button(OnSave)
            {
                text = "Save"
            };

            Button loadButton = new Button(OnLoad)
            {
                text = "Load"
            };

            Button renameAllElementsButton = new Button(OnRenameAllElement)
            {
                text = "Rename All Elements"
            };

            toolbar.Add(m_fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(renameAllElementsButton);

            rootVisualElement.Add(toolbar);
        }

        private void AddGraphView()
        {
            m_dsGraphView = new DSGraphView(this);
            m_dsGraphView.StretchToParentSize();
            rootVisualElement.Add(m_dsGraphView);
        }

        private void AddStyle()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void OnSave()
        {
            m_dsGraphView.SaveData();
            DSUtils.SaveGraph(m_dsGraphView.DSData, m_fileNameTextField.value);
        }

        private void OnLoad()
        {
            DSData dsData = DSUtils.LoadGraph(m_fileNameTextField.value);
            m_dsGraphView.LoadData(dsData);
        }

        private void OnRenameAllElement()
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
