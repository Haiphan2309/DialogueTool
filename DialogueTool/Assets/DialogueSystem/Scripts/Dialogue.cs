using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueChoice
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

        private bool canEditNextIndex
        {
            get
            {
                if (DialogueContainer == null) return true;
                return DialogueContainer.dialogueNodes.Count > 0 &&
                       DialogueContainer.dialogueNodes[DialogueContainer.dialogueNodes.Count - 1] != this;
            }
        }

        [NonSerialized] public Dialogue DialogueContainer;
    }

    [Serializable]
    public class Dialogue : ISerializationCallbackReceiver
    {
        public List<DialogueNode> dialogueNodes = new List<DialogueNode>();

        public void OnBeforeSerialize()
        {
            UpdateIndexes();
        }

        public void OnAfterDeserialize()
        {
        }

        private void UpdateIndexes()
        {
            for (int i = 0; i < dialogueNodes.Count; i++)
            {
                dialogueNodes[i].Index = i;
                dialogueNodes[i].DialogueContainer = this;

                if (i == dialogueNodes.Count - 1)
                {
                    dialogueNodes[i].NextIndex = i + 1;
                }
            }
        }
    }
}
