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
        [SerializeField] private List<DSGroupData> _groupDatas; //1 group present for 1 NPC
        public List<DSGroupData> GroupDatas => _groupDatas; // read-only

        [SerializeField] private List<DSNodeData> _ungroupedNodeDatas;
        public List<DSNodeData> UngroupedNodeDatas => _ungroupedNodeDatas;

        [SerializeField] private DSNodeData _startNodeData;
        public DSNodeData StartNodeData
        {
            get => _startNodeData;
            set => _startNodeData = value;
        }

        public DSData()
        {
            _groupDatas = new List<DSGroupData>();
            _ungroupedNodeDatas = new List<DSNodeData>();
            _startNodeData = new DSNodeData();
        }

#if UNITY_EDITOR
        public void AddUngroupedNodeData(DSNodeData nodeData)
        {
            //Debug.Log("Add ungroup node data: " + nodeData.GetHashCode());
            nodeData.Index = GetNodeCount();
            _ungroupedNodeDatas.Add(nodeData);
        }

        public void RemoveUngroupedNodeData(DSNodeData nodeData)
        {
            //Debug.Log("Remove ungroup node data: " + nodeData.GetHashCode());
            _ungroupedNodeDatas.Remove(nodeData);
            ReIndexNodeData();
        }

        public void AddGroupData(DSGroupData groupData)
        {
            //Debug.Log("Add group data " + groupData.Index);
            groupData.Index = _groupDatas.Count;
            _groupDatas.Add(groupData);
        }

        public void RemoveGroupData(DSGroupData groupData)
        {
            //Debug.Log("Remove Group Data " + groupData.Index);
            _groupDatas.Remove(groupData);
            ReIndexGroupData();
        }

        public void ReIndexGroupData()
        {
            for (int i = 0; i < _groupDatas.Count; i++)
            {
                _groupDatas[i].Index = i;
            }
        }

        public void ReIndexNodeData()
        {
            int nodeCount = 0;

            foreach (var node in _ungroupedNodeDatas)
            {
                node.Index = nodeCount++;
            }

            foreach (var group in _groupDatas)
            {
                foreach (var node in group.NodeDatas)
                {
                    node.Index = nodeCount++;
                }
            } 
        }

        public int GetNodeCount()
        {
            int nodeCount = _ungroupedNodeDatas.Count;

            foreach (var group in _groupDatas)
            {
                nodeCount += group.NodeDatas.Count;
            }

            return nodeCount;
        }

        public void SetStartNodeData(DSNodeData nodeData)
        {
            _startNodeData = nodeData;
        }
#endif
    }

    [Serializable]
    public class DSGroupData
    {
        [SerializeField] private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField] private int _index;
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        [SerializeField] private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        [SerializeField] private List<DSNodeData> _nodeDatas;
        public List<DSNodeData> NodeDatas => _nodeDatas;

        public DSGroupData()
        {
            _nodeDatas = new List<DSNodeData>();
        }

#if UNITY_EDITOR
        public void AddNodeData(DSNodeData nodeData)
        {
            //Debug.Log("Add Node data " + nodeData.GetHashCode());
            nodeData.GroupDataIndex = _index;
            _nodeDatas.Add(nodeData);
        }

        public void RemoveNodeData(DSNodeData nodeData)
        {
            //Debug.Log("Remove node data: " + nodeData.GetHashCode());
            nodeData.GroupDataIndex = -1;
            _nodeDatas.Remove(nodeData);
        }
#endif
    }

    [Serializable]
    public class DSNodeData
    {
        [SerializeField] private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField] private int _index;
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        [SerializeField] private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        [SerializeField] private int _nextNodeIndex;
        public int NextNodeIndex
        {
            get => _nextNodeIndex;
            set => _nextNodeIndex = value;
        }

        [SerializeField] private List<DSChoiceData> _choiceDatas;
        public List<DSChoiceData> ChoiceDatas
        {
            get => _choiceDatas;
            set => _choiceDatas = value;
        }

        [SerializeField] private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField] private TextBoxType _textBoxType;
        public TextBoxType TextBoxType
        {
            get => _textBoxType;
            set => _textBoxType = value;
        }

        [SerializeField] private TalkingEmotion _talkingEmotion;
        public TalkingEmotion TalkingEmotion
        {
            get => _talkingEmotion;
            set => _talkingEmotion = value;
        }

        [SerializeField] private bool _isHaveCallBack;
        public bool IsHaveCallBack
        {
            get => _isHaveCallBack;
            set => _isHaveCallBack = value;
        }

        [SerializeField] private int _groupDataIndex;
        public int GroupDataIndex
        {
            get => _groupDataIndex;
            set => _groupDataIndex = value;
        }

        public DSNodeData()
        {
            _choiceDatas = new List<DSChoiceData>();
            _nextNodeIndex = -1;
            _groupDataIndex = -1;
        }

#if UNITY_EDITOR
        public void AddChoiceData(DSChoiceData choiceData)
        {
            //Debug.Log("Add choice data " + choiceData);
            _choiceDatas.Add(choiceData);
        }

        public void RemoveChoiceData(DSChoiceData choiceData)
        {
            //Debug.Log("Remove choice data " + choiceData);
            _choiceDatas.Remove(choiceData);
        }
#endif
    }

    [Serializable]
    public class DSChoiceData
    {
        [SerializeField] private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField] private int _nextNodeIndex;
        public int NextNodeIndex
        {
            get => _nextNodeIndex;
            set => _nextNodeIndex = value;
        }

        public DSChoiceData(string text)
        {
            _nextNodeIndex = -1;
            _text = text;
        }
    }

    public class SODialogue : ScriptableObject
    {
        [SerializeField] private DSData _dsData;
        public DSData DSData
        {
            get => _dsData;
            set => _dsData = value;
        }

        /// <summary>
        /// This is the Start Node in graph, this node don't have any text data, just have a output port to the first node.
        /// </summary>
        /// <returns></returns>
        public DSNodeData GetStartNodeData()
        {
            foreach (var nodeData in _dsData.UngroupedNodeDatas)
            {
                if (nodeData.Index == 0)
                {
                    return nodeData;
                }
            }

            foreach (var groupData in _dsData.GroupDatas)
            {
                foreach (var nodeData in groupData.NodeDatas)
                {
                    if (nodeData.Index == 0)
                    {
                        return nodeData;
                    }
                }
            }

            return null;
        }

        public DSNodeData FindDSNodeDataBy(int index)
        {
            if (index == -1)
            {
                return null;
            }

            foreach (var groupData in _dsData.GroupDatas)
            {
                foreach (var nodeData in groupData.NodeDatas)
                {
                    if (nodeData.Index == index)
                    {
                        return nodeData;
                    }
                }
            }

            foreach(var nodeData in _dsData.UngroupedNodeDatas)
            {
                if (nodeData.Index == index)
                {
                    return nodeData;
                }
            }

            return null;
        }
    }
}
