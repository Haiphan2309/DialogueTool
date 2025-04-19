using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace DialogueSystem
{
    public enum DialogueState
    {
        FINISH,
        TALKING,
        PAUSE,
        CHOOSING,
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
        [SerializeField] private UIChoosingTextBox m_uiChoosingTextBox;

        [SerializeField] private Color m_nameColor;

        private Coroutine m_talkCor;

        bool m_isTalkingSpeedUp;
        bool m_isSkipTalking;

        private void Awake()
        {
            Instance = this;

            m_dialogueState = DialogueState.FINISH;
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_dialogueState)
            {
                case DialogueState.TALKING:
                    if (Input.GetKeyDown(KeyCode.F)) //watch out this case, it's might be call right after SetDialogue()
                    {
                        SkipTalking();
                    }
                    if (Input.GetKey(KeyCode.F))
                    {
                        m_isTalkingSpeedUp = true;
                    }
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        m_isTalkingSpeedUp = false;
                    }
                    break;
                case DialogueState.CHOOSING:
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        m_uiChoosingTextBox.ActiveUpChoice();
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        m_uiChoosingTextBox.ActiveDownChoice();
                    }
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        ChooseChoice();
                        DisplayDialogue();
                    }
                    break;
                case DialogueState.PAUSE:
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        DisplayDialogue();
                    }
                    break;
                default:
                    break;
            }

            if (m_dialogueState != DialogueState.FINISH)
            {
                int updateDialogueIndex = Mathf.Clamp(m_currentDialogueIndex, 0, m_dialogues.Count - 1); 
                m_uiChoosingTextBox.UpdatePos(m_dialogues[updateDialogueIndex].ObjectTransform.position);
                m_uiDialogueTextBox.UpdatePos(m_dialogues[updateDialogueIndex].ObjectTransform.position);
            }
        }

        public void SetDialogue(List<Dialogue> dialogues)
        {
            m_dialogues = dialogues;

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

            m_uiDialogueTextBox.Setup("", node.TextBoxType, m_dialogues[m_currentDialogueIndex].ObjectTransform.position);

            if (m_talkCor != null)
            {
                StopCoroutine(m_talkCor);
            }
            m_talkCor = StartCoroutine(CorTypeSentence(node));
        }

        private void ToNextNodeIndex()
        {
            m_currentNodeIndex = m_dialogues[m_currentDialogueIndex].DialogueNodes[m_currentNodeIndex].NextIndex;
            if (m_currentNodeIndex >= m_dialogues[m_currentDialogueIndex].DialogueNodes.Count)
            {
                m_currentDialogueIndex++;
                m_currentNodeIndex = 0;
            }
        }

        private void CheckDialogueEvent()
        {
            foreach (var dialogueEvent in m_dialogues[m_currentDialogueIndex].DialogueEvents)
            {
                if (dialogueEvent.Index == m_currentNodeIndex)
                {
                    dialogueEvent.Event?.Invoke();
                    break;
                }
            }
        }

        private void ActiveChoosing(List<DialogueChoice> choices)
        {
            m_uiChoosingTextBox.Setup(choices, m_uiDialogueTextBox, m_dialogues[m_currentDialogueIndex].ObjectTransform.position);
            m_dialogueState = DialogueState.CHOOSING;
        }

        private void ChooseChoice()
        {
            DialogueChoice currentChoice = m_uiChoosingTextBox.GetCurrentChoice();
            m_currentNodeIndex = currentChoice.NextIndex;

            m_uiChoosingTextBox.OnChooseChoice();
        }

        public void EndDialogue()
        {
            m_dialogueState = DialogueState.FINISH;
            m_uiDialogueTextBox.Hide();
        }

        private void EndTalking()
        {
            m_dialogueState = DialogueState.PAUSE;

            List<DialogueChoice> choices = m_dialogues[m_currentDialogueIndex].DialogueNodes[m_currentNodeIndex].Choices;
            if (choices.Count > 0)
            {
                ActiveChoosing(choices);
            }
            else
            {
                CheckDialogueEvent();
                ToNextNodeIndex();
            }
        }

        private void SkipTalking()
        {
            if (m_talkCor != null)
            {
                StopCoroutine(m_talkCor);
                m_talkCor = null;
            }

            m_uiDialogueTextBox.SetText(m_dialogues[m_currentDialogueIndex].DialogueNodes[m_currentNodeIndex].Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_uiDialogueTextBox.GetComponent<RectTransform>());
        }
        private IEnumerator CorTypeSentence(DialogueNode node)
        {
            m_dialogueState = DialogueState.TALKING;
            string str = "";
            m_uiDialogueTextBox.SetText(str);
            foreach (char letter in node.Text.ToCharArray())
            {
                if (letter == '&')
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(m_nameColor);
                    //str += $"<color=#{hexColor}>{SaveLoadManager.Instance.GameData.PlayerName}</color>";
                }
                else
                {
                    str += letter;
                }

                float sec = 0.02f; //default
                if (m_isTalkingSpeedUp)
                {
                    sec = 0.01f;
                }

                if (letter == '.' || letter == ',' || letter == '!' || letter == '?')
                    yield return new WaitForSecondsRealtime(sec * 5);
                else
                    yield return new WaitForSecondsRealtime(sec);

                m_uiDialogueTextBox.SetText(str);
            }

            EndTalking();
        }
    }
}
