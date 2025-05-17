using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Windows
{
    public class DSGroupData
    {
        public int Index { get; set; }
        public List<DSNodeData> NodeDatas { get; private set; }

        public DSGroupData() 
        {
            NodeDatas = new List<DSNodeData>();
        }
        public void AddNodeData(DSNodeData nodeData)
        {
            Debug.Log("Add Node data ");
            NodeDatas.Add(nodeData);
        }

        public void RemoveNodeData(DSNodeData nodeData)
        {
            Debug.Log("Remove node data ");
            NodeDatas.Remove(nodeData);
        }
    }

    public class DSGroup : Group
    {
        public DSGroupData GroupData { get; set; }
        public void Setup(string titleValue, Vector2 position)
        {
            GroupData = new DSGroupData();
            title = titleValue;
            SetPosition(new Rect(position, Vector2.zero));
        }

        public void ReupdateNameByIndex()
        {
            title = "Group " + GroupData.Index;
        }
    }
}
