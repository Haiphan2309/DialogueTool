using UnityEngine;
using DialogueSystem;

public class NPC : BaseNPC
{
    bool _skipLastEndDialogueFrame = false;

    //This function appear just for input testing suppose, feel free to remove it! (For example: Create a class managing all the input!)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && DialogueManager.Instance.DialogueState == DialogueState.FINISH)
        {
            if (!_skipLastEndDialogueFrame)
            {
                _skipLastEndDialogueFrame = true;
                Talk();
            }
            else
            {
                _skipLastEndDialogueFrame = false;
            }
        }
    }

    public void Talk()
    {
        DialogueManager.Instance.SetDialogue(_soDialogue, _talkingNPCDatas);
    }
}
