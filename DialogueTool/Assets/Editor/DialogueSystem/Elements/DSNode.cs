using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Data;
using System.ComponentModel;

namespace DialogueSystem.Windows
{
    public class DSNode : Node
    {
        public DSNodeData NodeData { get; set; }

        private VisualElement _customDataContainer;
        private VisualElement _choiceContainer;

        protected DSGraphView _graphView;

        public DSNode()
        {
            //This constructor is unused
        }

        public DSNode(Vector2 position, DSGraphView graphView)
        {
            Setup(position, graphView);
            Draw();
        }
        
        protected void Setup(Vector2 position, DSGraphView graphView)
        {
            _graphView = graphView;
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
            title = "New Node";

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

            EnumField textBoxTypeField = new EnumField("Text Box Type", NodeData.TextBoxType);
            textBoxTypeField.RegisterValueChangedCallback(evt =>
            {
                NodeData.TextBoxType = (TextBoxType)evt.newValue;
            });

            EnumField emotionField = new EnumField("Emotion", NodeData.TalkingEmotion);
            emotionField.RegisterValueChangedCallback(evt =>
            {
                NodeData.TalkingEmotion = (TalkingEmotion)evt.newValue;
            });

            _customDataContainer.Add(textBoxTypeField);
            _customDataContainer.Add(emotionField);

            Button addChoiceButton = new Button(() => AddChoice(new DSChoiceData("An example choosing text.")))
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

        virtual public void SaveData()
        {
            NodeData.Name = title;
            NodeData.Position = GetPosition().position;

            foreach (var child in _customDataContainer.Children())
            {
                if (child is Foldout foldout && foldout.name == "DialogueTextFoldout")
                {
                    var textField = foldout.Q<TextField>();
                    if (textField != null)
                    {
                        NodeData.Text = textField.value; // not work
                    }
                }
            }
                
            //todo update text for choice
        }

        virtual public void LoadData(DSNodeData nodeData)
        {
            SetPosition(new Rect(nodeData.Position, Vector2.zero));
            title = nodeData.Name;
            foreach (var child in _customDataContainer.Children())
            {
                if (child is Foldout foldout && foldout.name == "DialogueTextFoldout")
                {
                    var textField = foldout.Q<TextField>();
                    if (textField != null)
                    {
                        textField.value = nodeData.Text;
                    }
                }

                if (child is EnumField enumField)
                {
                    if (enumField.label == "Text Box Type")
                    {
                        enumField.SetValueWithoutNotify(nodeData.TextBoxType);
                    }

                    if (enumField.label == "Emotion")
                    {
                        enumField.SetValueWithoutNotify(nodeData.TalkingEmotion);
                    }
                }
            }

            _choiceContainer.Clear();
            foreach (var choiceData in nodeData.ChoiceDatas)
            {
                AddChoice(choiceData);
            }

            NodeData = nodeData;

            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddChoice(DSChoiceData choiceData)
        {
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
                _graphView.DeleteElements(outputPort.connections);
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
            DisconnectAllInputPorts();
            DisconnectAllOutputPorts();
        }

        public void DisconnectAllInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        public void DisconnectAllOutputPorts()
        {
            DisconnectPorts(outputContainer);
            DisconnectPorts(_choiceContainer);
        }

        public void RemoveOutputPortData(Port portToRemoveData)
        {
            foreach (Port port in outputContainer.Children())
            {
                if (port == portToRemoveData)
                {
                    NodeData.NextNodeData = null;
                    return;
                }
            }

            for (int i = 0; i < _choiceContainer.childCount; i++)
            {
                Port port = _choiceContainer.ElementAt(i) as Port;
                if (portToRemoveData == port)
                {
                    NodeData.ChoiceDatas[i].NextNodeData = null;
                }
            }
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
