using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class ChoiceData
    {
        public string Text {  get; set; }

        public ChoiceData(string text)
        {
            Text = text;
        }
    }
    public class DSNode : Node
    {
        public string NodeName { get; set; }
        public List<ChoiceData> ChoiceDatas { get; set; }
        public string Text { get; set; }

        private VisualElement _customDataContainer;
        private VisualElement _choiceContainer;
        public void Setup(Vector2 position)
        {
            NodeName = "NodeName";
            Text = "This is an example text.";
            ChoiceDatas = new List<ChoiceData>();

            SetPosition(new Rect(position, Vector2.zero));
            AddStyle();

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public void Draw()
        {
            /* Title Container */

            Label nodeNameTextField = new Label(NodeName);
            nodeNameTextField.AddToClassList("ds-node__text-field");
            nodeNameTextField.AddToClassList("ds-node__text-field__hidden");
            nodeNameTextField.AddToClassList("ds-node__filename-text-field");
            titleContainer.Insert(0, nodeNameTextField);            

            /* Input Container */

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Prev node";
            inputContainer.Add(inputPort);

            /* Output Container */

            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Next node";
            outputContainer.Add(outputPort);
            outputContainer.SetEnabled(true);

            /* Extensions Container */

            _customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textField = new TextField()
            {
                value = Text,
                multiline = true
            };
            //textField.AddToClassList("ds-node__text-field");
            textField.AddToClassList("ds-node__quote-text-field");

            textFoldout.Add(textField);
            _customDataContainer.Add(textFoldout);

            Button addChoiceButton = new Button(AddChoice)
            {
                text = "Add Choice"
            };
            _customDataContainer.Add(addChoiceButton);

            _choiceContainer = new VisualElement();
            _choiceContainer.AddToClassList("ds-node__choice-container");

            _customDataContainer.Add(_choiceContainer);
            _customDataContainer.AddToClassList("ds-node__custom-data-container");

            extensionContainer.Add(_customDataContainer);

            RefreshExpandedState();
        }

        private void AddChoice()
        {
            ChoiceData choiceData = new ChoiceData("An example choosing text.");
            ChoiceDatas.Add(choiceData);
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Choice " + ChoiceDatas.Count;
            TextField choiceTextField = new TextField()
            {
                value = choiceData.Text
            };
            choiceTextField.AddToClassList("ds-node__choice-text-field");

            Button deleteChoiceButton = new Button(()=>
            {
                _choiceContainer.Remove(outputPort);
                ChoiceDatas.Remove(choiceData);

                if (ChoiceDatas.Count == 0)
                {
                    outputContainer.SetEnabled(true);
                }
                ReupdateChoicePortName();
                RefreshExpandedState();
            })
            {
                text = "X"
            };

            outputPort.Add(choiceTextField);
            outputPort.Add(deleteChoiceButton);

            _choiceContainer.Add(outputPort);

            outputContainer.SetEnabled(false);
            RefreshExpandedState();
        }

        private void AddStyle()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSNodeStyles.uss");
            styleSheets.Add(styleSheet);
        }

        private void ReupdateChoicePortName()
        {
            int index = 1;
            foreach (var child in _choiceContainer.Children())
            {
                Port port = child as Port;
                if (port != null)
                {
                    port.portName = "Choice " + index;
                    index++;
                }
            }
        }
    }
}
