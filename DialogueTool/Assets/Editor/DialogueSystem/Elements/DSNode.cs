using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class DSNodeData
    {
        public int Index { get; set; }
        public Vector2 Position { get; set; }
        public DSNodeData NextNodeData { get; set; }
        public List<DSChoiceData> ChoiceDatas { get; set; }
        public string Text { get; set; }
        public DSGroupData GroupData { get; set; }

        public DSNodeData()
        {
            ChoiceDatas = new List<DSChoiceData>();
        }

        public void AddChoiceData(DSChoiceData choiceData)
        {
            Debug.Log("Add choice data " + choiceData);
            ChoiceDatas.Add(choiceData);
        }

        public void RemoveChoiceData(DSChoiceData choiceData)
        {
            Debug.Log("Remove choice data " + choiceData);
            ChoiceDatas.Remove(choiceData);
        }

    }
    public class DSChoiceData
    {
        public string Text { get; set; }
        public DSNodeData NextNodeData { get; set; }

        public DSChoiceData(string text)
        {
            Text = text;
        }
    }
    public class DSNode : Node
    {
        public string NodeName { get; set; }
        public DSNodeData NodeData { get; set; }

        private VisualElement _customDataContainer;
        private VisualElement _choiceContainer;

        protected DSGraphView _graphView;

        public DSNode()
        {
            //Do not use this constructor
        }

        public DSNode(Vector2 position, DSGraphView graphView)
        {
            Setup(position, graphView);
            Draw();
        }
        
        protected void Setup(Vector2 position, DSGraphView graphView)
        {
            _graphView = graphView;
            NodeName = "New Node";
            NodeData = new DSNodeData();
            NodeData.Text = "This is an example text.";
            NodeData.ChoiceDatas = new List<DSChoiceData>();

            SetPosition(new Rect(position, Vector2.zero));
            AddStyle();

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        protected virtual void Draw()
        {
            /* Title Container */

            //Label nodeNameTextField = new Label(NodeName);
            //nodeNameTextField.AddToClassList("ds-node__text-field");
            //nodeNameTextField.AddToClassList("ds-node__text-field__hidden");
            //nodeNameTextField.AddToClassList("ds-node__filename-text-field");
            //titleContainer.Insert(0, nodeNameTextField);            
            title = NodeName;

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
                value = NodeData.Text,
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
            DSChoiceData choiceData = new DSChoiceData("An example choosing text.");
            NodeData.AddChoiceData(choiceData);
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputPort.portName = "Choice " + NodeData.ChoiceDatas.Count;
            TextField choiceTextField = new TextField()
            {
                value = choiceData.Text
            };
            choiceTextField.AddToClassList("ds-node__choice-text-field");

            Button deleteChoiceButton = new Button(()=>
            {
                _choiceContainer.Remove(outputPort);
                NodeData.RemoveChoiceData(choiceData);

                if (NodeData.ChoiceDatas.Count == 0)
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

        public void ReupdateNameByIndex()
        {
            title = "Node " + NodeData.Index;
        }

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                _graphView.DeleteElements(port.connections);
            }
        }
    }
}
