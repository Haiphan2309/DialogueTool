using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GDC.Utils
{
    public class UIUtils : MonoBehaviour
    {
        public static void LockInput()
        {
            EventSystem e = FindObjectOfType<EventSystem>();
            if (e != null)  
            {
                e.enabled = false;
            }
        }
        public static void UnlockInput()
        {
            EventSystem e = FindObjectOfType<EventSystem>();
            if (e != null)  
            {
                e.enabled = true;
            }   
        }
    }
}
