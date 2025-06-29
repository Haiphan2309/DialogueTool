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

            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnElementsDeleted();

            Init();
        }

        private void Init()
        {
            DSStartNode startNode = new DSStartNode(new Vector2(100, 100), this);
            AddElement(startNode);
            DSData.AddUngroupNodeData(startNode.NodeData);

            DSGroup group= CreateGroup(new Vector2(230, 150));
            AddElement(group);

            DSNode node = CreateNode(new Vector2(230, 150));
            AddElement(node);

            group.AddElement(node);
        }

        private void OnGroupElementsRemoved()
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
                    DSData.AddUngroupNodeData(dsNode.NodeData);
                }
            };
        }

        private void OnGroupElementsAdded()
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
                    DSData.RemoveUngroupNodeData(dsNode.NodeData);
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
                        if (node.NodeData.GroupData != null)
                        {
                            DSGroup dsGroup = FindDSGroupBy(node.NodeData.GroupData);
                            //this dsGroup should not null!
                            dsGroup.RemoveElement(node);
                        }
                        DSData.RemoveUngroupNodeData(node.NodeData);
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
            this.AddManipulator(CreateUnGroupContextualMenu());
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

        private IManipulator CreateUnGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Ungroup", actionEvent => UnGroup())
            );

            return contextualMenuManipulator;
        }

        private DSNode CreateNode(Vector2 position)
        {
            DSNode node = new DSNode(position, this);

            DSData.AddUngroupNodeData(node.NodeData);

            foreach (GraphElement element in selection)
            {
                if (element is DSGroup)
                {
                    DSGroup group = (DSGroup)element;
                    group.AddElement(node);
                    group.GroupData.AddNodeData(node.NodeData);
                    DSData.RemoveUngroupNodeData(node.NodeData);
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
                    DSData.RemoveUngroupNodeData(node.NodeData);
                }
            }    

            return group;
        }

        private void UnGroup()
        {
            foreach (GraphElement element in selection)
            {
                if (element is DSNode node && node.NodeData.GroupData != null)
                {
                    DSGroup group = FindDSGroupBy(node.NodeData.GroupData);
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

        public DSGroup FindDSGroupBy(DSGroupData groupData)
        {
            foreach (var element in graphElements)
            {
                if (element is DSGroup group && group.GroupData == groupData)
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

            foreach (var nodeData in dsData.UngroupNodeDatas)
            {
                if (nodeData.Index == 0) //this is start node
                {
                    Debug.Log("Create start node");
                    DSStartNode startNode = new DSStartNode(DSData.StartNodeData.Position, this);
                    startNode.LoadData(dsData.StartNodeData);
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

                    Debug.Log("nodeindex: " + dsNode.NodeData.Index + " have choice port count: " + dsNode.GetAllChoicePorts().Count);
                    for (int i = 0; i < dsNode.GetAllChoicePorts().Count; i++)
                    {
                        Debug.Log("Find choice ports with index " + i + " of nodeindex: " + dsNode.NodeData.Index + " and next node data: " + dsNode.NodeData.NextNodeIndex);
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
