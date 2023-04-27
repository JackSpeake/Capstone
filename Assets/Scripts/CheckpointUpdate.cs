using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUpdate : MonoBehaviour
{
    [SerializeField] private int checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("POG");
        if (other.CompareTag("Player"))
        {
            Debug.Log("POG");
            other.GetComponent<NetworkCharacterMovementHandler>().checkpoint = checkpoint;
        }
    }
}
