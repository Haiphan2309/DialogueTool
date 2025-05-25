using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using DialogueSystem.Data;

//You can customize this class like the way you like
public class BaseNPC : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected List<Dialogue> _dialogues;
    public void DoTalkAnim(TalkingEmotion emotion)
    {
        if (_animator == null)
        {
            return;
        }

        switch (emotion)
        {
            case TalkingEmotion.HAPPY:
                _animator.Play("happy_talking");
                break;
            case TalkingEmotion.ANGRY:
                _animator.Play("angry_talking");
                break;
            case TalkingEmotion.SAD:
                _animator.Play("sad_talking");
                break;
            case TalkingEmotion.SURPRISE:
                _animator.Play("surprise_talking");
                break;
            case TalkingEmotion.THINKING:
                _animator.Play("thinking_talking");
                break;
            default:
                _animator.Play("idle_talking");
                break;
        }
    }

    public void StopTalkAnim()
    {
        if (_animator == null)
        {
            return;
        }

        _animator.Play("idle");
    }
}
