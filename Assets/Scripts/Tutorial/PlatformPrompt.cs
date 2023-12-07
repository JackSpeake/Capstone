using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformPrompt : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private TMPro.TMP_Text text;

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("LFG");
        text.text = message;
    }

    private void OnTriggerExit(Collider other)
    {
        text.text = "";
    }
}
