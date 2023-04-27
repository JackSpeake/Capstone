using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public float teleportHeight = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkCharacterMovementHandler nCMH = other.GetComponent<NetworkCharacterMovementHandler>();
            Vector3 newPos = nCMH.transform.position;
            newPos.y = teleportHeight;
            nCMH.GetComponent<NetworkCharacterControllerPrototype>().VelMult = 0;

            nCMH.TeleportToPosition(newPos);
        }
    }
}
