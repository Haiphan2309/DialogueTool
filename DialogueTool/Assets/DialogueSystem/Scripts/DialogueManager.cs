using DialogueSystem.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem.Config;

namespace DialogueSystem.UI
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

        [SerializeField] private TextBoxConfig _textBoxConfig;
        public TextBoxConfig TextBoxConfig { get => _textBoxConfig; }

        private DialogueState _dialogueState;
        public DialogueState DialogueState
        {
            get => _dialogueState;
            private set => _dialogueState = value;
        }

        private SODialogue _soDialogue;
        private DSNodeData _currentNodeData;
        private List<TalkingNPCData> _talkingNPCDatas;

        [SerializeField] private RectTransform _dialogueContainer;

        private UIDialogueTextBox _uiDialogueTextBox;
        private UIChoosingTextBox _uiChoosingTextBox;

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
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        SkipTalking();
                    }
                    if (Input.GetKey(KeyCode.Z))
                    {
                        _isTalkingSpeedUp = true;
                    }
                    if (Input.GetKeyUp(KeyCode.Z))
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
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        ChooseChoice();
                        DisplayDialogue();
                    }
                    break;
                case DialogueState.PAUSE:
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        DisplayDialogue();
                    }
                    break;
                default:
                    break;
            }

            if (_dialogueState != DialogueState.FINISH)
            {
                TalkingNPCData talkingNPCData = GetCurrentTalkingNPCData(_currentNodeData);
                if (talkingNPCData != null)
                {
                    if (!talkingNPCData.CenterTransform)
                    {
                        talkingNPCData.CenterTransform = talkingNPCData.BaseNPC ? talkingNPCData.BaseNPC.transform : null;
                    }

                    if (talkingNPCData.CenterTransform)
                    {
                        if (_uiDialogueTextBox)
                        {
                            _uiDialogueTextBox.UpdatePos(talkingNPCData.CenterTransform.transform.position, talkingNPCData.Size);
                        }

                        if (_uiChoosingTextBox)
                        {
                            _uiChoosingTextBox.UpdatePos(talkingNPCData.CenterTransform.transform.position, talkingNPCData.Size);
                        }
                    }
                }
                else
                {
                    if (_uiDialogueTextBox)
                    {
                        _uiDialogueTextBox.UpdatePos();
                    }

                    if (_uiChoosingTextBox)
                    {
                        _uiChoosingTextBox.UpdatePos();
                    }
                }
            }
        }

        public void SetDialogue(SODialogue soDialogue, List<TalkingNPCData> talkingNPCDatas = null)
        {
            _soDialogue = soDialogue;
            _talkingNPCDatas = talkingNPCDatas;

            _currentNodeData = _soDialogue.GetStartNodeData();

            if (_currentNodeData != null)
            {
                _dialogueState = DialogueState.TALKING;
                DisplayDialogue();
            }
        }

        public void DisplayDialogue()
        {
            if (_dialogueState != DialogueState.CHOOSING)
            {
                _currentNodeData = _soDialogue.FindDSNodeDataBy(_currentNodeData.NextNodeIndex);
            }

            if (_currentNodeData == null)
            {
                EndDialogue();
                return;
            }

            if (_uiDialogueTextBox != null)
            {
                Destroy(_uiDialogueTextBox.gameObject);
            }
            _uiDialogueTextBox = Instantiate(_textBoxConfig.UIDialogueTextBoxPrefab, _dialogueContainer);

            TalkingNPCData talkingNPCData = GetCurrentTalkingNPCData(_currentNodeData);
            if (talkingNPCData != null)
            {
                if (!talkingNPCData.CenterTransform)
                {
                    talkingNPCData.CenterTransform = talkingNPCData.BaseNPC ? talkingNPCData.BaseNPC.transform : null;
                }
                Vector3 objectPos = talkingNPCData.CenterTransform ? talkingNPCData.CenterTransform.transform.position : Vector3.zero;
                _uiDialogueTextBox.Setup("", _currentNodeData.TextBoxType, objectPos, talkingNPCData.Size);

                if (talkingNPCData.BaseNPC != null)
                {
                    talkingNPCData.BaseNPC.DoTalkAnim(_currentNodeData.TalkingEmotion);
                }
            }
            else
            {
                _uiDialogueTextBox.Setup("", _currentNodeData.TextBoxType);
            }

            if (_talkCor != null)
            {
                StopCoroutine(_talkCor);
            }
            _talkCor = StartCoroutine(CorTypeSentence(_currentNodeData));
        }

        /// <summary>
        /// This use for trigger event after the node having event just end
        /// </summary>
        private void CheckDialogueEvent()
        {
            //Not implement yet!

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
            if (_uiChoosingTextBox != null)
            {
                Destroy(_uiChoosingTextBox.gameObject);
            }
            _uiChoosingTextBox = Instantiate(_textBoxConfig.ChoosingTextBoxConfig.UIChoosingTextBoxPrefab, _dialogueContainer);

            TalkingNPCData talkingNPCData = GetCurrentTalkingNPCData(_currentNodeData);
            if (talkingNPCData != null)
            {
                if (!talkingNPCData.CenterTransform)
                {
                    talkingNPCData.CenterTransform = talkingNPCData.BaseNPC ? talkingNPCData.BaseNPC.transform : null;
                }
                Vector3 objectPos = talkingNPCData.CenterTransform ? talkingNPCData.CenterTransform.transform.position : Vector3.zero;
                _uiChoosingTextBox.Setup(choiceDatas, _uiDialogueTextBox, objectPos, talkingNPCData.Size);
            }
            else
            {
                _uiChoosingTextBox.Setup(choiceDatas, _uiDialogueTextBox);
            }
            
            _dialogueState = DialogueState.CHOOSING;
        }

        private void ChooseChoice()
        {
            DSChoiceData currentChoiceData = _uiChoosingTextBox.GetCurrentChoiceData();
            _currentNodeData = _soDialogue.FindDSNodeDataBy(currentChoiceData.NextNodeIndex);

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

            TalkingNPCData talkingNPCData = GetCurrentTalkingNPCData(_currentNodeData);
            if (talkingNPCData != null && talkingNPCData.BaseNPC != null)
            {
                talkingNPCData.BaseNPC.StopTalkAnim();
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

            bool isActiveSpecialTextColor = false;

            foreach (char letter in nodeData.Text.ToCharArray())
            {
                str += letter;

                float sec = _textBoxConfig.DefaultTalkingSpeedDelay;
                if (_isTalkingSpeedUp)
                {
                    sec = _textBoxConfig.FastTalkingSpeedDelay;
                }

                if (letter == '.' || letter == ',' || letter == '!' || letter == '?')
                    yield return new WaitForSecondsRealtime(sec * _textBoxConfig.SlowTalkingSpeedDelayMutil);
                else
                    yield return new WaitForSecondsRealtime(sec);

                _uiDialogueTextBox.SetText(str);
            }

            EndTalking();
        }

        private TalkingNPCData GetCurrentTalkingNPCData(DSNodeData nodeData)
        {
            if (nodeData.GroupDataIndex == -1)
            {
                return null;
            }

            if (nodeData.GroupDataIndex >= _talkingNPCDatas.Count)
            {
                Debug.LogError("Out of range when passing groupDataIndex >= talkingNPCDatas.Count");
                return null;
            }

            return _talkingNPCDatas[nodeData.GroupDataIndex];
        }
    }
}
