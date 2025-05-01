using Sirenix.Serialization;
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

        private DialogueState _dialogueState;
        public DialogueState DialogueState
        {
            get => _dialogueState;
            private set => _dialogueState = value;
        }

        private List<Dialogue> _dialogues; //1 dialogue is representing for 1 NPC

        private int _currentDialogueIndex;
        private int _currentNodeIndex; //current dialogue node index
        [SerializeField] private UIDialogueTextBox _uiDialogueTextBox;
        [SerializeField] private UIChoosingTextBox _uiChoosingTextBox;

        [SerializeField] private Color _nameColor;

        private Coroutine _talkCor;

        bool _isTalkingSpeedUp;
        bool _isSkipTalking;

        private void Awake()
        {
            Instance = this;

            _dialogueState = DialogueState.FINISH;
        }

        // Update is called once per frame
        void Update()
        {
            switch (_dialogueState)
            {
                case DialogueState.TALKING:
                    if (Input.GetKeyDown(KeyCode.F)) //watch out this case, it's might be call right after SetDialogue()
                    {
                        SkipTalking();
                    }
                    if (Input.GetKey(KeyCode.F))
                    {
                        _isTalkingSpeedUp = true;
                    }
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        _isTalkingSpeedUp = false;
                    }
                    break;
                case DialogueState.CHOOSING:
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        _uiChoosingTextBox.ActiveUpChoice();
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        _uiChoosingTextBox.ActiveDownChoice();
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

            if (_dialogueState != DialogueState.FINISH)
            {
                int updateDialogueIndex = Mathf.Clamp(_currentDialogueIndex, 0, _dialogues.Count - 1);
                TalkingObjectData talkingObjectData = _dialogues[updateDialogueIndex].TalkingObjectData;
                if (!talkingObjectData.CenterTransform)
                {
                    talkingObjectData.CenterTransform = talkingObjectData.BaseNPC ? talkingObjectData.BaseNPC.transform : null;
                }

                if (talkingObjectData.CenterTransform)
                {
                    _uiChoosingTextBox.UpdatePos(talkingObjectData.CenterTransform.transform.position, talkingObjectData.Size);
                    _uiDialogueTextBox.UpdatePos(talkingObjectData.CenterTransform.transform.position, talkingObjectData.Size);
                }
            }
        }

        public void SetDialogue(List<Dialogue> dialogues)
        {
            _dialogues = dialogues;

            _dialogueState = DialogueState.TALKING;
            _currentDialogueIndex = 0;
            _currentNodeIndex = 0;

            DisplayDialogue();
            //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
        }

        public void SetDialogue(Dialogue dialogue)
        {
            _dialogues = new List<Dialogue>();
            _dialogues.Add(dialogue);

            _dialogueState = DialogueState.TALKING;
            _currentDialogueIndex = 0;
            _currentNodeIndex = 0;

            DisplayDialogue();
            //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
        }

        public void DisplayDialogue()
        {
            //not do first dialog
            if ((_currentDialogueIndex != 0 || _currentNodeIndex != 0) && _dialogueState != DialogueState.CHOOSING)
            {
                ToNextNodeIndex();
            }

            if (_currentDialogueIndex >= _dialogues.Count || _currentNodeIndex >= _dialogues[_currentDialogueIndex].DialogueNodes.Count)
            {
                EndDialogue();
                return;
            }

            DialogueNode node = _dialogues[_currentDialogueIndex].DialogueNodes[_currentNodeIndex];

            TalkingObjectData talkingObjectData = _dialogues[_currentDialogueIndex].TalkingObjectData;
            if (!talkingObjectData.CenterTransform)
            {
                talkingObjectData.CenterTransform = talkingObjectData.BaseNPC ? talkingObjectData.BaseNPC.transform : null;
            }
            Vector3 objectPos = talkingObjectData.CenterTransform ? talkingObjectData.CenterTransform.transform.position : Vector3.zero;
            _uiDialogueTextBox.Setup("", node.TextBoxType, objectPos, talkingObjectData.Size);

            if (_talkCor != null)
            {
                StopCoroutine(_talkCor);
            }
            _talkCor = StartCoroutine(CorTypeSentence(node));
        }

        private void ToNextNodeIndex()
        {
            _currentNodeIndex = _dialogues[_currentDialogueIndex].DialogueNodes[_currentNodeIndex].NextIndex;
            if (_currentNodeIndex >= _dialogues[_currentDialogueIndex].DialogueNodes.Count)
            {
                _currentDialogueIndex++;
                _currentNodeIndex = 0;
            }
        }

        private void CheckDialogueEvent()
        {
            foreach (var dialogueEvent in _dialogues[_currentDialogueIndex].DialogueEvents)
            {
                if (dialogueEvent.Index == _currentNodeIndex)
                {
                    dialogueEvent.Event?.Invoke();
                    break;
                }
            }
        }

        private void ActiveChoosing(List<DialogueChoice> choices)
        {
            TalkingObjectData talkingObjectData = _dialogues[_currentDialogueIndex].TalkingObjectData;
            if (!talkingObjectData.CenterTransform)
            {
                talkingObjectData.CenterTransform = talkingObjectData.BaseNPC ? talkingObjectData.BaseNPC.transform : null;
            }
            Vector3 objectPos = talkingObjectData.CenterTransform ? talkingObjectData.CenterTransform.transform.position : Vector3.zero;
            _uiChoosingTextBox.Setup(choices, _uiDialogueTextBox, objectPos, talkingObjectData.Size);
            _dialogueState = DialogueState.CHOOSING;
        }

        private void ChooseChoice()
        {
            DialogueChoice currentChoice = _uiChoosingTextBox.GetCurrentChoice();
            _currentNodeIndex = currentChoice.NextIndex;

            _uiChoosingTextBox.OnChooseChoice();
        }

        public void EndDialogue()
        {
            _dialogueState = DialogueState.FINISH;
            _uiDialogueTextBox.Hide();
        }

        private void EndTalking()
        {
            _dialogueState = DialogueState.PAUSE;

            List<DialogueChoice> choices = _dialogues[_currentDialogueIndex].DialogueNodes[_currentNodeIndex].Choices;
            if (choices.Count > 0)
            {
                ActiveChoosing(choices);
            }
            else
            {
                CheckDialogueEvent();
            }
        }

        private void SkipTalking()
        {
            if (_talkCor != null)
            {
                StopCoroutine(_talkCor);
                _talkCor = null;
            }

            _uiDialogueTextBox.SetText(_dialogues[_currentDialogueIndex].DialogueNodes[_currentNodeIndex].Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_uiDialogueTextBox.GetComponent<RectTransform>());
            EndTalking();
        }
        private IEnumerator CorTypeSentence(DialogueNode node)
        {
            _dialogueState = DialogueState.TALKING;
            string str = "";
            _uiDialogueTextBox.SetText(str);
            foreach (char letter in node.Text.ToCharArray())
            {
                if (letter == '&')
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(_nameColor);
                    //str += $"<color=#{hexColor}>{SaveLoadManager.Instance.GameData.PlayerName}</color>";
                }
                else
                {
                    str += letter;
                }

                float sec = 0.02f; //default
                if (_isTalkingSpeedUp)
                {
                    sec = 0.01f;
                }

                if (letter == '.' || letter == ',' || letter == '!' || letter == '?')
                    yield return new WaitForSecondsRealtime(sec * 8);
                else
                    yield return new WaitForSecondsRealtime(sec);

                _uiDialogueTextBox.SetText(str);
            }

            EndTalking();
        }
    }
}
