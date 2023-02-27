using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("The strength of the explosion (I think this only applies to self use)")]
    public float explosionForce = 20f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 0f)
        {
            Vector3 stayGrounded = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.position = stayGrounded;
        }
    }

    public void ExplodeBomb(float explosionRadius)
    {
        Vector3 impactPoint = this.transform.position;
        Collider[] objectsHit = Physics.OverlapSphere(impactPoint, explosionRadius);
        foreach (Collider other in objectsHit)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"Bomb position: {transform.position}");
                Debug.Log($"Player Position: {other.transform.position}");
                float distance = Vector3.Distance(impactPoint, other.transform.position);
                float adjustedExplosionForce = (explosionRadius / (2 * distance)) * explosionForce;
                Vector3 launchDirection = other.transform.position - impactPoint;
                launchDirection.Normalize();
                Debug.Log(launchDirection);
                Debug.Log(adjustedExplosionForce);
                other.gameObject.GetComponent<NetworkCharacterMovementHandler>().AddForce(launchDirection, adjustedExplosionForce);
                Debug.Log("Launching Player");
            }
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GROUND"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
