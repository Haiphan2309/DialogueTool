using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem.Data;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Android;

namespace DialogueSystem.Windows
{
    public class DSGraphView : GraphView
    {
        private DSEditorWindow _editorWindow;
        public DSData DSData;
        public DSGraphView(DSEditorWindow editorWindow)
        {
            DSData = new DSData();
            _editorWindow = editorWindow;

            AddGridBackground();
            AddStyle();
            AddManipulators();

            OngroupedElementsAdded();
            OngroupedElementsRemoved();
            OnElementsDeleted();
            OnElementsCutOrCopy();
            OnElementsPatse();

            Init();
        }

        private void Init()
        {
            DSStartNode startNode = new DSStartNode(new Vector2(100, 100), this);
            AddElement(startNode);
            DSData.AddUngroupedNodeData(startNode.NodeData);

            DSGroup group= CreateGroup(new Vector2(230, 150));
            AddElement(group);

            DSNode node = CreateNode(new Vector2(230, 150));
            AddElement(node);

            group.AddElement(node);
        }

        private void OnElementsCutOrCopy()
        {
            serializeGraphElements = (elements) =>
            {
                Debug.Log("COPY");
                var nodes = elements.OfType<DSNode>().ToList();
                var groups = elements.OfType<DSGroup>().ToList();

                List<string> data = new();

                foreach (var node in nodes)
                {
                    node.SaveData();
                    data.Add(JsonUtility.ToJson(new Wrapper<DSNodeData>("DSNodeData", node.NodeData)));
                }

                foreach (var group in groups)
                {
                    group.SaveData();
                    data.Add(JsonUtility.ToJson(new Wrapper<DSGroupData>("DSGroupData", group.GroupData)));
                }

                return string.Join("\n", data); // This is what gets stored in Unity's internal clipboard
            };
        }

        private void OnElementsPatse()
        {
            unserializeAndPaste = (operationName, dataStr) =>
            {
                Debug.Log("PASTE");
                string[] dataLines = dataStr.Split('\n');

                List<DSNode> groupedNodes = new List<DSNode>();

                foreach (var line in dataLines)
                {
                    Debug.Log("line: " + line);
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    Wrapper<DSNodeData> nodeWrapper = JsonUtility.FromJson<Wrapper<DSNodeData>>(line);
                    if (nodeWrapper != null && nodeWrapper.Type == "DSNodeData")
                    {
                        var nodeData = nodeWrapper.Data;
                        nodeData.Name += " (Copy)";
                        nodeData.Position += new Vector2(30, 30); // offset to avoid overlap

                        if (nodeData.Index == 0) //this is start node
                        {
                            Debug.Log("Can't copy start node");
                            continue;
                        }

                        //remove all port link
                        nodeData.NextNodeIndex = -1;
                        foreach (var choiceData in nodeData.ChoiceDatas)
                        {
                            choiceData.NextNodeIndex = -1;
                        }

                        DSNode newNode = CreateNode(nodeData.Position);
                        var newIndex = newNode.NodeData.Index;
                        var newPosition = newNode.NodeData.Position;
                        newNode.LoadData(nodeData); //It's change index and position equal copyed element
                        newNode.NodeData.Index = newIndex;
                        newNode.NodeData.Position = newPosition;
                        
                        AddElement(newNode);

                        if (nodeData.GroupDataIndex != -1)
                        {
                            groupedNodes.Add(newNode);
                        }

                        continue;
                    }

                    Wrapper<DSGroupData> groupWrapper = JsonUtility.FromJson<Wrapper<DSGroupData>>(line);
                    if (groupWrapper != null && groupWrapper.Type == "DSGroupData")
                    {
                        var groupData = groupWrapper.Data;
                        groupData.Name += " (Copy)";
                        groupData.Position += new Vector2(30, 30);

                        var oldIndex = groupData.Index;

                        DSGroup newGroup = CreateGroup(groupData.Position);
                        var newIndex = newGroup.GroupData.Index;
                        var newPosition = newGroup.GroupData.Position;
                        newGroup.LoadData(groupData); //It's change index and position equal copyed element
                        newGroup.GroupData.Index = newIndex;
                        newGroup.GroupData.Position = newPosition;
                        AddElement(newGroup);

                        groupData.NodeDatas.Clear();

                        for (int i = groupedNodes.Count - 1; i >= 0; i--)
                        {
                            var node = groupedNodes[i];
                            if (node.NodeData.GroupDataIndex == oldIndex)
                            {
                                newGroup.AddElement(node);
                                groupedNodes.RemoveAt(i);
                            }
                        }
                    }
                }

                //After all node were grouped, remain nodes in groupedNodes don't find the group will be reset groupDataIndex
                //(because the missing group doesn't copied;
                foreach (var node in groupedNodes)
                {
                    node.NodeData.GroupDataIndex = -1;
                }
            };

        }

        private void OngroupedElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode) || element is DSStartNode)
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup)group;
                    DSNode dsNode = (DSNode)element;

                    dsGroup.GroupData.RemoveNodeData(dsNode.NodeData);
                    DSData.AddUngroupedNodeData(dsNode.NodeData);
                }
            };
        }

        private void OngroupedElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is DSStartNode)
                    {
                        group.RemoveElement(element);
                        continue;
                    }
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup)group;
                    DSNode dsNode = (DSNode)element;

                    dsGroup.GroupData.AddNodeData(dsNode.NodeData);
                    DSData.RemoveUngroupedNodeData(dsNode.NodeData);
                }
            };
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                List<ISelectable> tempSelectables = selection.ToList();
                foreach (GraphElement selectedElement in tempSelectables)
                {
                    if (selectedElement is DSGroup group)
                    {
                        foreach (var member in group.containedElements.ToList())
                        {
                            group.RemoveElement(member);
                        }
                        DSData.RemoveGroupData(group.GroupData);
                        DSData.ReIndexGroupData();
                    }

                    if (selectedElement is DSStartNode)
                    {
                        continue;
                        //can't delete start node
                    }

                    if (selectedElement is DSNode node)
                    {
                        if (node.NodeData.GroupDataIndex != -1)
                        {
                            DSGroup dsGroup = FindDSGroupBy(node.NodeData.GroupDataIndex); //this dsGroup should not null!
                            dsGroup.RemoveElement(node);
                        }
                        DSData.RemoveUngroupedNodeData(node.NodeData);
                        DSData.ReIndexNodeData();

                        node.DisconnectAllPorts();
                    }

                    if (selectedElement is Edge edge)
                    {
                        DSNode inputNode = (DSNode)edge.input.node;
                        DSNode outputNode = (DSNode)edge.output.node;

                        outputNode.RemoveOutputPortData(edge.output);
                        DeleteElements(edge.output.connections);
                    }

                    RemoveElement(selectedElement);
                }
            };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu());
            this.AddManipulator(CreateGroupContextualMenu());
            this.AddManipulator(CreateUngroupedContextualMenu());
        }

        private void AddStyle()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSGraphViewStyles.uss");
            styleSheets.Add(styleSheet);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground); //ensure it's always render first
        }

        private IManipulator CreateNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateUngroupedContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Ungrouped", actionEvent => Ungrouped())
            );

            return contextualMenuManipulator;
        }

        private DSNode CreateNode(Vector2 position)
        {
            DSNode node = new DSNode(position, this);

            DSData.AddUngroupedNodeData(node.NodeData);

            foreach (GraphElement element in selection)
            {
                if (element is DSGroup)
                {
                    DSGroup group = (DSGroup)element;
                    group.AddElement(node);
                    group.GroupData.AddNodeData(node.NodeData);
                    DSData.RemoveUngroupedNodeData(node.NodeData);
                }
            }

            return node;
        }

        private DSGroup CreateGroup(Vector2 position)
        {
            DSGroup group = new DSGroup(position, this);

            DSData.AddGroupData(group.GroupData);

            foreach (GraphElement element in selection)
            {
                if (element is DSNode && !(element is DSStartNode))
                {
                    DSNode node = (DSNode)element;
                    group.AddElement(node);
                    group.GroupData.AddNodeData(node.NodeData);
                    DSData.RemoveUngroupedNodeData(node.NodeData);
                }
            }    

            return group;
        }

        private void Ungrouped()
        {
            foreach (GraphElement element in selection)
            {
                if (element is DSNode node && node.NodeData.GroupDataIndex != -1)
                {
                    DSGroup group = FindDSGroupBy(node.NodeData.GroupDataIndex);
                    //group should not be null
                    group.RemoveElement(node);
                }
            }
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, mousePosition - _editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void RenameAllElement()
        {
            foreach (var element in graphElements)
            {
                if (element is DSGroup group)
                {
                    group.ReupdateNameByIndex();
                }

                if (element is DSStartNode)
                {
                    continue;
                }

                if (element is DSNode node)
                {
                    node.ReupdateNameByIndex();
                }
            }    
        }

        public DSNode FindDSNodeBy(int index)
        {
            if (index == -1)
            {
                return null;
            }

            foreach (var element in graphElements)
            {
                if (element is DSNode node && node.NodeData.Index == index)
                {
                    return node;
                }
            }
            return null;
        }

        public DSGroup FindDSGroupBy(int index)
        {
            foreach (var element in graphElements)
            {
                if (element is DSGroup group && group.GroupData.Index == index)
                {
                    return group;
                }
            }
            return null;
        }

        public void ClearData()
        {
            DeleteElements(graphElements.ToList());
            DSData = new DSData();
        }

        public void SaveData()
        {
            foreach(var element in graphElements)
            {
                if (element is DSGroup dsGroup)
                {
                    dsGroup.SaveData();
                }

                if (element is DSNode dsNode)
                {
                    dsNode.SaveData();
                }

                if (element is DSStartNode dsStartNode)
                {
                    DSData.SetStartNodeData(dsStartNode.NodeData);
                }
            }
        }

        public void LoadData(DSData dsData)
        {
            ClearData();

            foreach (var nodeData in dsData.UngroupedNodeDatas)
            {
                if (nodeData.Index == 0) //this is start node
                {
                    Debug.Log("Create start node");
                    DSStartNode startNode = new DSStartNode(nodeData.Position, this);
                    startNode.LoadData(nodeData);
                    AddElement(startNode);
                    continue;
                }

                DSNode dsNode = CreateNode(nodeData.Position);
                dsNode.LoadData(nodeData);
                AddElement(dsNode);
            }

            foreach (var groupData in dsData.GroupDatas)
            {
                DSGroup dsGroup = CreateGroup(groupData.Position);

                foreach (var nodeData in groupData.NodeDatas)
                {
                    DSNode dsNode = CreateNode(nodeData.Position);
                    dsNode.LoadData(nodeData);
                    AddElement(dsNode);
                    dsGroup.AddElement(dsNode);
                }

                dsGroup.LoadData(groupData);
                AddElement(dsGroup);
            }

            //Load port connection, do this after done creating all nodes
            foreach(var element in graphElements)
            {
                if (element is DSNode dsNode)
                {
                    Port outputPort = dsNode.GetOutputPort();

                    DSNode nextNode = FindDSNodeBy(dsNode.NodeData.NextNodeIndex);
                    if (nextNode != null)
                    {
                        Port inputPort = nextNode.GetInputPort();
                        if (inputPort != null)
                        {
                            Edge edge = outputPort.ConnectTo(inputPort);
                            AddElement(edge);
                        }
                    }

                    //Debug.Log("nodeindex: " + dsNode.NodeData.Index + " have choice port count: " + dsNode.GetAllChoicePorts().Count);
                    for (int i = 0; i < dsNode.GetAllChoicePorts().Count; i++)
                    {
                        //Debug.Log("Find choice ports with index " + i + " of nodeindex: " + dsNode.NodeData.Index + " and next node data: " + dsNode.NodeData.NextNodeIndex);
                        DSNode choiceNextNode = FindDSNodeBy(dsNode.NodeData.ChoiceDatas[i].NextNodeIndex);
                        if (choiceNextNode == null)
                        {
                            continue;
                        }

                        Port choicePort = dsNode.GetAllChoicePorts()[i];
                        Port choiceInputPort = choiceNextNode.GetInputPort();
                        Edge edge = choicePort.ConnectTo(choiceInputPort);
                        AddElement(edge);
                    }
                }
            }

            DSData = dsData;
        }
    }
}
