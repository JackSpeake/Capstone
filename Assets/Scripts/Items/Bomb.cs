using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bomb : MonoBehaviour
{
    [Tooltip("The strength of the explosion (I think this only applies to self use)")]
    public float explosionForce = 20f;

    // Variables related to displaying an indicator for the explosion range of the bomb
    [Tooltip("Number of segments used to display the explosion radius indicator")]
    public int segments = 360;
    private float width = 0.25f;
    private LineRenderer line;

    private float explosionRadius;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.startWidth = width;
        line.endWidth = width;
        Debug.Log(line);
    }

    public void SetExplosionRadius(float radius)
    {
        this.explosionRadius = radius;
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

    public void ExplodeBomb()
    {
        if (gameObject.activeInHierarchy)
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
        }

        if (NetworkPlayer.Local.Runner.IsServer)
            NetworkPlayer.Local.Runner.Despawn(this.GetComponent<NetworkObject>());

        Destroy(this);
    }

    public void CreateExplosionRadiusIndicator()
    {
        float x;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * explosionRadius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * explosionRadius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GROUND"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
