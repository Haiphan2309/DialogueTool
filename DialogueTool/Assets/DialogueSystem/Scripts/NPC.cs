using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using Sirenix.OdinInspector;

public class NPC : MonoBehaviour
{
    [SerializeField] private List<Dialogue> dialogues;

    [Button]
    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(dialogues);
    }
}
