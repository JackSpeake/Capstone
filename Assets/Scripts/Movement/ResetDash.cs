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
            this.GetComponentInChildren<ParticleSystem>().Stop();
            AudioSource.PlayClipAtPoint(pickup, gameObject.transform.position);
            StartCoroutine("CauseStartAgain");
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
        this.GetComponentInChildren<ParticleSystem>().Play();
        this.GetComponent<MeshRenderer>().enabled = true;
    }
}
