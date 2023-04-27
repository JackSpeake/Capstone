using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDash : MonoBehaviour
{
    public AudioClip pickup;
    public float waitBeforeRespawn = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkCharacterMovementHandler nCMH = other.GetComponent<NetworkCharacterMovementHandler>();
            nCMH.DashResetFunc();
            this.GetComponent<MeshRenderer>().enabled = false;
            //AudioSource.PlayClipAtPoint(pickup, gameObject.transform.position);
        }
    }

    private IEnumerator CauseStartAgain()
    {
        yield return new WaitForSeconds(waitBeforeRespawn);
        Respawn();
    }

    public void Respawn()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
}
