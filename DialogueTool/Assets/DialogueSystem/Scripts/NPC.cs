using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using DialogueSystem.Data;

public class NPC : BaseNPC
{
    [SerializeField] private List<Dialogue> dialogues;
    [SerializeField] private SODialogue _soDialogue;

    [Button]
    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(_soDialogue);
    }
}
