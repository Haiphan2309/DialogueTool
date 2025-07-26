using UnityEngine;
using DialogueSystem;

namespace DialogueSystem.UI
{
    public class SampleNPC : BaseNPC
    {
        private bool _skipLastEndDialogueFrame = false;

        //This function appear just for input testing suppose, feel free to remove it! (For example: Create a class managing all the input!)
        //Using LateUpdate() to avoid skip dialogue animation in the first dialogue due to DialogueManager.cs also having GetKeyDown(KeyCode.Z) in it's Update()
        private void LateUpdate()
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
}
