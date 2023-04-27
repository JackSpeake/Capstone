using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public float teleportHeight = 5;
    [SerializeField] public List<Vector3> checkpoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkCharacterMovementHandler nCMH = other.GetComponent<NetworkCharacterMovementHandler>();
            Vector3 newPos = checkpoints[nCMH.checkpoint];
            newPos.y = teleportHeight;
            nCMH.GetComponent<NetworkCharacterControllerPrototype>().VelMult = 0;

            nCMH.TeleportToPosition(newPos);
        }
    }
}
