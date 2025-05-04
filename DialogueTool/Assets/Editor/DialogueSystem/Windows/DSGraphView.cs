using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class DSGraphView : GraphView
    {
        private DSEditorWindow _editorWindow;
        public DSGraphView(DSEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;

            AddGridBackground();
            AddStyle();
            AddManipulators();

            deleteSelection = OnDeleteSelection;
        }

        private void OnDeleteSelection(string operationName, AskUser askUser)
        {
            var elementsToRemove = selection.ToList();

            foreach (var element in elementsToRemove)
            {
                if (element is Group group)
                {
                    foreach (var member in group.containedElements.ToList())
                    {
                        group.RemoveElement(member);
                    }
                }

                RemoveElement((Group)element);
            }
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
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }

        private DSNode CreateNode(Vector2 position)
        {
            DSNode node = new DSNode();

            node.Setup(position);
            node.Draw();

            return node;
        }

        private Group CreateGroup(Vector2 position)
        {
            DSGroup group = new DSGroup();

            group.Setup("New Group", position);
            group.Draw();

            foreach (GraphElement element in selection)
            {
                if (element is DSNode)
                {
                    DSNode node = (DSNode)element;
                    group.AddElement(node);
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
    }
}
