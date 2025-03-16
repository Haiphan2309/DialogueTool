using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioPlayer
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        public AudioSource audioSource;
        public bool IsLowPriority = false;
    }
}
