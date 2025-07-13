using UnityEngine;
using DialogueSystem;

public class NPC : BaseNPC
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && DialogueManager.Instance.DialogueState == DialogueState.FINISH)
        {
            Talk();
        }
    }

    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(_soDialogue, _talkingNPCDatas);
    }
}
