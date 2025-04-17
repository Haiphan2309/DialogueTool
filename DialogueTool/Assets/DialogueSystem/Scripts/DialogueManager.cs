using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public enum DialogueState
    {
        FINISH,
        TALKING,
        PAUSE,
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private DialogueState m_dialogueState;
        public DialogueState DialogueState 
        {
            get => m_dialogueState;
            private set => m_dialogueState = value;
        }

        private List<Dialogue> m_dialogues; //1 dialogue is representing for 1 NPC

        private int m_currentDialogueIndex;
        private int m_currentNodeIndex; //current dialogue node index
        [SerializeField] private UIDialogueTextBox m_uiDialogueTextBox;

        [SerializeField] private Color m_nameColor;

        private Coroutine m_talkCor;

        float sec;
        bool isChoosingBranch1, isBranchDialogue, isBlackChoosingText;
        bool m_isTalkingSpeedUp;

        private void Awake()
        {
            Instance = this;

            m_dialogueState = DialogueState.FINISH;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && m_dialogueState != DialogueState.TALKING)
            {
                //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
                DisplayDialogue();
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                //todo: do choosing stuff

                // if (choosingSymbol.gameObject.activeSelf)
                //     SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
                // isChoosingBranch1 = !isChoosingBranch1;
                // if (isChoosingBranch1)
                //     choosingSymbol.rectTransform.anchoredPosition = new Vector2(0,choosingText1.rectTransform.anchoredPosition.y);
                // else
                //     choosingSymbol.rectTransform.anchoredPosition = new Vector2(0, choosingText2.rectTransform.anchoredPosition.y);
            }

            if (Input.GetKey(KeyCode.F))
            {
                m_isTalkingSpeedUp = true;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                m_isTalkingSpeedUp = false;
            }
        }

        public void SetDialogue(List<Dialogue> dialogues)
        {
            m_dialogues = dialogues;

            sec = 0.02f;
            m_dialogueState = DialogueState.TALKING;
            m_currentDialogueIndex = 0;
            m_currentNodeIndex = 0;

            DisplayDialogue();
            //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
        }

        public void SetDialogue(Dialogue dialogue)
        {
            m_dialogues = new List<Dialogue>();
            m_dialogues.Add(dialogue);

            sec = 0.02f;
            m_dialogueState = DialogueState.TALKING;
            m_currentDialogueIndex = 0;
            m_currentNodeIndex = 0;

            DisplayDialogue();
            //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
        }

        public void DisplayDialogue()
        {
            if (m_currentDialogueIndex >= m_dialogues.Count || m_currentNodeIndex >= m_dialogues[m_currentDialogueIndex].DialogueNodes.Count)
            {
                EndDialogue();
                return;
            }

            DialogueNode node = m_dialogues[m_currentDialogueIndex].DialogueNodes[m_currentNodeIndex];

            if (m_talkCor != null)
            {
                StopCoroutine(m_talkCor);
            }
            m_uiDialogueTextBox.Setup(node.Text, node.TextBoxType, m_dialogues[m_currentDialogueIndex].ObjectTransform.position);
            m_talkCor = StartCoroutine(CorTypeSentence(node));

            m_currentNodeIndex++;
            if (m_currentNodeIndex >= m_dialogues[m_currentDialogueIndex].DialogueNodes.Count)
            {
                m_currentDialogueIndex++;
                m_currentNodeIndex = 0;
            }

            // for (int i = 0; i < eventTalks.Count; i++)
            // {
            //     if (eventIndexs[i] == sentenceIndex)
            //     {
            //         eventTalks[i]?.Invoke();
            //     }
            // }
        }

        public void EndDialogue()
        {
            m_dialogueState = DialogueState.FINISH;
        }
        void ActiveChoosing()
        {
            // choosingText1.gameObject.SetActive(true);
            // choosingText2.gameObject.SetActive(true);
            // choosingSymbol.gameObject.SetActive(true);
            // choosingSymbol.rectTransform.anchoredPosition = new Vector2(0, choosingText1.rectTransform.anchoredPosition.y);
            // isChoosingBranch1 = true;
        }
        private IEnumerator CorTypeSentence(DialogueNode node)
        {
            m_dialogueState = DialogueState.TALKING;
            int i = 0;
            string text = "";
            m_uiDialogueTextBox.UpdateText(text);
            foreach (char letter in node.Text.ToCharArray())
            {
                if (letter == '&')
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(m_nameColor);
                    //text += $"<color=#{hexColor}>{SaveLoadManager.Instance.GameData.PlayerName}</color>";
                }
                else
                {
                    text += letter;
                }

                if (m_isTalkingSpeedUp == false || (i % 4 == 0 && m_isTalkingSpeedUp))
                {
                    if (letter == '.' || letter == ',' || letter == '!' || letter == '?')
                        yield return new WaitForSecondsRealtime(sec * 5);
                    else
                        yield return new WaitForSecondsRealtime(sec);
                }
                m_uiDialogueTextBox.UpdateText(text);
                i++;
            }
            m_dialogueState = DialogueState.PAUSE;

            if (node.Choices.Count > 0)
            {
                ActiveChoosing();
            }
        }
    }
}
