using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public class NPC : BaseNPC
{
    [SerializeField] private List<Dialogue> dialogues;

    [Button]
    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(dialogues);
    }
}
