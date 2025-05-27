using DialogueSystem.Data;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private SODialogue _soDialogue;
        private DSNodeData _firstNodeData;
        private DSNodeData _currentNodeData;

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
                TalkingObjectData talkingObjectData = _soDialogue.GetCurrentTalkingObjectData(_currentNodeData);
                if (talkingObjectData != null)
                {
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
                else
                {
                    //todo: a default position for _uidDialogueTextbox
                }
            }
        }

        public void SetDialogue(SODialogue soDialogue)
        {
            _soDialogue = soDialogue;

            _currentNodeData = _soDialogue.GetStartNodeData();

            if (_currentNodeData != null)
            {
                _dialogueState = DialogueState.TALKING;
                DisplayDialogue();
            }

            //SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_INTERACT);
        }

        public void DisplayDialogue()
        {
            if (_dialogueState != DialogueState.CHOOSING) //may be not to need this line
            {
                _currentNodeData = _currentNodeData.NextNodeData;
            }

            if (_currentNodeData == null)
            {
                EndDialogue();
                return;
            }

            TalkingObjectData talkingObjectData = _soDialogue.GetCurrentTalkingObjectData(_currentNodeData);
            if (talkingObjectData != null)
            {
                if (!talkingObjectData.CenterTransform)
                {
                    talkingObjectData.CenterTransform = talkingObjectData.BaseNPC ? talkingObjectData.BaseNPC.transform : null;
                }
                Vector3 objectPos = talkingObjectData.CenterTransform ? talkingObjectData.CenterTransform.transform.position : Vector3.zero;
                _uiDialogueTextBox.Setup("", _currentNodeData.TextBoxType, objectPos, talkingObjectData.Size);
            }
            else
            {
                Debug.Log("Talking data object is null");
                //todo
            }

            if (_talkCor != null)
            {
                StopCoroutine(_talkCor);
            }
            _talkCor = StartCoroutine(CorTypeSentence(_currentNodeData));
        }

        private void CheckDialogueEvent()
        {
            //foreach (var dialogueEvent in _dialogues[_currentDialogueIndex].DialogueEvents)
            //{
            //    if (dialogueEvent.Index == _currentNodeIndex)
            //    {
            //        dialogueEvent.Event?.Invoke();
            //        break;
            //    }
            //}
        }

        private void ActiveChoosing(List<DSChoiceData> choiceDatas)
        {
            TalkingObjectData talkingObjectData = _soDialogue.GetCurrentTalkingObjectData(_currentNodeData);
            if (talkingObjectData != null)
            {
                if (!talkingObjectData.CenterTransform)
                {
                    talkingObjectData.CenterTransform = talkingObjectData.BaseNPC ? talkingObjectData.BaseNPC.transform : null;
                }
                Vector3 objectPos = talkingObjectData.CenterTransform ? talkingObjectData.CenterTransform.transform.position : Vector3.zero;
                _uiChoosingTextBox.Setup(choiceDatas, _uiDialogueTextBox, objectPos, talkingObjectData.Size);
            }
            else
            {
                Debug.Log("Talking object data is null when active choosing");
                //todo
            }
            
            _dialogueState = DialogueState.CHOOSING;
        }

        private void ChooseChoice()
        {
            DSChoiceData currentChoiceData = _uiChoosingTextBox.GetCurrentChoiceData();
            _currentNodeData = currentChoiceData.NextNodeData;

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

            List<DSChoiceData> choiceDatas = _currentNodeData.ChoiceDatas;
            if (choiceDatas.Count > 0)
            {
                ActiveChoosing(choiceDatas);
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

            _uiDialogueTextBox.SetText(_currentNodeData.Text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_uiDialogueTextBox.GetComponent<RectTransform>());
            EndTalking();
        }
        private IEnumerator CorTypeSentence(DSNodeData nodeData)
        {
            _dialogueState = DialogueState.TALKING;
            string str = "";
            _uiDialogueTextBox.SetText(str);
            foreach (char letter in nodeData.Text.ToCharArray())
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
