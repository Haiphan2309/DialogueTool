using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcllusionCulling : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.SetActive(false);
    }
}
