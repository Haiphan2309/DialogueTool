using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class DSNode : Node
    {
        public string NodeName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }

        public void Setup(Vector2 position)
        {
            NodeName = "NodeName";
            Text = "This is an example text.";
            Choices = new List<string>();
            Choices.Add("Next Node");

            SetPosition(new Rect(position, Vector2.zero));
        }

        public void Draw()
        {
            /* Title Container */

            Label nodeNameTextField = new Label(NodeName);
            titleContainer.Insert(0, nodeNameTextField);            

            /* Input Container */

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Prev node";
            inputContainer.Add(inputPort);

            /* Output Container */

            foreach (string choice in Choices)
            {
                Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                outputPort.portName = "";

                //Button deleteChoiceButton = new Button()
                //{
                //    text = "X"
                //};

                //TextField choiceTextField = new TextField()
                //{
                //    value = choice
                //};

                //outputPort.Add(choiceTextField);
                //outputPort.Add(deleteChoiceButton);

                outputContainer.Add(outputPort);
            }

            /* Extensions Container */

            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textField = new TextField()
            {
                value = Text
            };

            textFoldout.Add(textField);
            customDataContainer.Add(textFoldout);

            Button addChoiceButton = new Button()
            {
                text = "Add Choice"
            };
            customDataContainer.Add(addChoiceButton);

            Choices.Add("aaa");
            Choices.Add("bbb");
            Choices.Add("ccc");
            foreach (string choice in Choices)
            {
                Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                outputPort.portName = choice;

                //Button deleteChoiceButton = new Button()
                //{
                //    text = "X"
                //};

                TextField choiceTextField = new TextField()
                {
                    value = ""
                };

                outputPort.Add(choiceTextField);
                //outputPort.Add(deleteChoiceButton);

                customDataContainer.Add(outputPort);
            }

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
    }
}
