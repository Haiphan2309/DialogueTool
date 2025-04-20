using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    [Serializable]
    public struct DialogueChoice
    {
        public int NextIndex;
        public string Text;
    }

    [Serializable]
    public class DialogueNode
    {
        [ReadOnly] public int Index;
        [EnableIf(nameof(canEditNextIndex))] public int NextIndex;

        [TextArea] public string Text;
        public List<DialogueChoice> Choices = new List<DialogueChoice>();
        public TextBoxType TextBoxType;
        //public UnityEvent AfterEvent; //event occur when this node done talk

        private bool canEditNextIndex
        {
            get
            {
                if (DialogueContainer == null) return true;
                return DialogueContainer.DialogueNodes.Count > 0 &&
                       DialogueContainer.DialogueNodes[DialogueContainer.DialogueNodes.Count - 1] != this;
            }
        }

        [NonSerialized] public Dialogue DialogueContainer;
    }

    [Serializable]
    public struct DialogueEvent
    {
        public UnityEvent Event;
        public int Index;
    }

    [Serializable]
    public struct TalkingObjectData
    {
        public Transform ObjectTransform;
        public float Size;
    }

    [Serializable]
    public class Dialogue : ISerializationCallbackReceiver
    {
        public TalkingObjectData TalkingObjectData;
        public List<DialogueNode> DialogueNodes = new List<DialogueNode>();
        public List<DialogueEvent> DialogueEvents = new List<DialogueEvent>();

        public void OnBeforeSerialize()
        {
            UpdateIndexes();
        }

        public void OnAfterDeserialize()
        {
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < DialogueNodes.Count; i++)
            {
                DialogueNodes[i].Index = i;
                DialogueNodes[i].DialogueContainer = this;

                if (i == DialogueNodes.Count - 1)
                {
                    DialogueNodes[i].NextIndex = i + 1;
                }
            }
        }
    }
}
