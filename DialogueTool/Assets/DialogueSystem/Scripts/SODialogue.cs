using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem.Data
{
    public enum TalkingEmotion
    {
        IDLE,
        HAPPY,
        ANGRY,
        SAD,
        SURPRISE,
        THINKING,
    }

    [Serializable]
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
            Debug.Log("Add ungroup node data: " + nodeData.GetHashCode());
            nodeData.Index = GetNodeCount();
            UngroupNodeDatas.Add(nodeData);
        }

        public void RemoveUngroupNodeData(DSNodeData nodeData)
        {
            Debug.Log("Remove ungroup node data: " + nodeData.GetHashCode());
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

            foreach (var node in UngroupNodeDatas)
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

    [Serializable]
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

    [Serializable]
    public class DSNodeData
    {
        public int Index { get; set; }
        public Vector2 Position { get; set; }
        public DSNodeData NextNodeData { get; set; }
        public List<DSChoiceData> ChoiceDatas { get; set; }
        public string Text { get; set; }
        public TextBoxType TextBoxType { get; set; }
        public TalkingEmotion TalkingEmotion { get; set; }
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

    [Serializable]
    public class DSChoiceData
    {
        public string Text { get; set; }
        public DSNodeData NextNodeData { get; set; }

        public DSChoiceData(string text)
        {
            Text = text;
        }
    }

    public class SODialogue : ScriptableObject
    {
        public DSData DSData { get; set; }
    }
}
