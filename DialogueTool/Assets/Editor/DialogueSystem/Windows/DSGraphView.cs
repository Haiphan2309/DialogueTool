using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class DSData
    {
        public List<DSGroupData> GroupDatas { get; private set; }
        public List<DSNodeData> UngroupNodeDatas { get; private set; }

        public DSData()
        {
            GroupDatas = new List<DSGroupData>();
            UngroupNodeDatas = new List<DSNodeData>();
        }

        public void AddUngroupNodeData(DSNodeData nodeData)
        {
            Debug.Log("Add ungroup node data");
            nodeData.Index = GetNodeCount();
            UngroupNodeDatas.Add(nodeData);
        }

        public void RemoveUngroupNodeData(DSNodeData nodeData)
        {
            Debug.Log("Remove ungroup node data");
            UngroupNodeDatas.Remove(nodeData);
            ReIndexNodeData();
        }

        public void AddGroupData(DSGroupData groupData)
        {
            Debug.Log("Add group data " + groupData.Index);
            groupData.Index = GroupDatas.Count;
            GroupDatas.Add(groupData);
        }

        public void RemoveGroupData(DSGroupData groupData)
        {
            Debug.Log("Remove Group Data " + groupData.Index);
            GroupDatas.Remove(groupData);
            ReIndexGroupData();
        }

        public void ReIndexGroupData()
        {
            for (int i = 0; i < GroupDatas.Count; i++)
            {
                GroupDatas[i].Index = i;
            }
        }

        public void ReIndexNodeData()
        {
            int nodeCount = 0;

            foreach (var group in GroupDatas)
            {
                foreach (var node in group.NodeDatas)
                {
                    node.Index = nodeCount++;
                }
            }
            
            foreach(var node in UngroupNodeDatas)
            {
                node.Index = nodeCount++;
            }
        }

        public int GetNodeCount()
        {
            int nodeCount = UngroupNodeDatas.Count;

            foreach (var group in GroupDatas)
            {
                nodeCount += group.NodeDatas.Count;
            }

            return nodeCount;
        }
    }
    public class DSGraphView : GraphView
    {
        private DSEditorWindow _editorWindow;
        public DSData Data;
        public DSGraphView(DSEditorWindow editorWindow)
        {
            Data = new DSData();
            _editorWindow = editorWindow;

            AddGridBackground();
            AddStyle();
            AddManipulators();

            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnElementsDeleted();
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup)group;
                    DSNode dsNode = (DSNode)element;

                    dsGroup.GroupData.RemoveNodeData(dsNode.NodeData);
                    Data.AddUngroupNodeData(dsNode.NodeData);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup)group;
                    DSNode dsNode = (DSNode)element;

                    dsGroup.GroupData.AddNodeData(dsNode.NodeData);
                    Data.RemoveUngroupNodeData(dsNode.NodeData);
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
                        Data.RemoveGroupData(group.GroupData);
                        Data.ReIndexGroupData();
                    }

                    if (selectedElement is DSNode node)
                    {
                        Data.RemoveUngroupNodeData(node.NodeData);
                        Data.ReIndexNodeData();
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
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent =>
                {
                    DSGroup group = CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition));
                    AddElement(group);
                    Data.AddGroupData(group.GroupData);
                }
            ));

            return contextualMenuManipulator;
        }

        private DSNode CreateNode(Vector2 position)
        {
            DSNode node = new DSNode();

            node.Setup(position);
            node.Draw();

            return node;
        }

        private DSGroup CreateGroup(Vector2 position)
        {
            DSGroup group = new DSGroup();

            group.Setup("New Group", position);

            foreach (GraphElement element in selection)
            {
                if (element is DSNode)
                {
                    DSNode node = (DSNode)element;
                    group.AddElement(node);
                    group.GroupData.AddNodeData(node.NodeData);
                    Data.RemoveUngroupNodeData(node.NodeData);
                }
            }    

            return group;
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

                if (element is DSNode node)
                {
                    node.ReupdateNameByIndex();
                }
            }    
        }
    }
}
