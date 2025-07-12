using DialogueSystem.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Windows
{
    public class DSStartNode : DSNode
    {
        public DSStartNode(Vector2 position, DSGraphView graphView)
        {
            Setup(position, _graphView);
            Draw();
        }

        protected override void Draw()
        {
            base.Draw();

            mainContainer.style.backgroundColor = new Color(0.1f, 0.5f, 1f, 0.4f);

            title = "Start";
            inputContainer.Clear();
            extensionContainer.Clear();
            RefreshExpandedState();
        }

        override public void SaveData()
        {
            base.SaveData();
        }

        override public void LoadData(DSNodeData nodeData)
        {
            SetPosition(new Rect(nodeData.Position, Vector2.zero));
            title = nodeData.Name;
            NodeData = nodeData;
        }
    }
}
