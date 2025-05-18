using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Windows
{
    public class DSGroupData
    {
        public int Index { get; set; }
        public Vector2 Position { get; set; }
        public List<DSNodeData> NodeDatas { get; private set; }

        public DSGroupData() 
        {
            NodeDatas = new List<DSNodeData>();
        }
        public void AddNodeData(DSNodeData nodeData)
        {
            Debug.Log("Add Node data " + nodeData.GetHashCode());
            nodeData.GroupData = this;
            NodeDatas.Add(nodeData);
        }

        public void RemoveNodeData(DSNodeData nodeData)
        {
            Debug.Log("Remove node data: " + nodeData.GetHashCode());
            nodeData.GroupData = null;
            NodeDatas.Remove(nodeData);
        }
    }

    public class DSGroup : Group
    {
        public DSGroupData GroupData { get; set; }

        protected DSGraphView _graphView;

        public DSGroup(Vector2 position, DSGraphView graphView)
        {
            Setup(position, graphView);
            Draw();
        }

        protected void Setup(Vector2 position, DSGraphView graphView)
        {
            _graphView = graphView;
            GroupData = new DSGroupData();
            SetPosition(new Rect(position, Vector2.zero));
        }

        protected void Draw()
        {
            title = "New NPC";
        }

        public void ReupdateNameByIndex()
        {
            title = "NPC " + GroupData.Index;
        }
    }
}
