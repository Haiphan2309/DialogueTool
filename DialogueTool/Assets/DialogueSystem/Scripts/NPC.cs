using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using UnityEngine.UIElements;
using DialogueSystem.Data;

public class NPC : BaseNPC
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Talk();
        }
    }

    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(_soDialogue, _talkingNPCDatas);
    }
}
