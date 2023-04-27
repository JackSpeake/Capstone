using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bomb : NetworkBehaviour
{
    [Tooltip("The strength of the explosion (I think this only applies to self use)")]
    public float explosionForce = 2f;

    // Variables related to displaying an indicator for the explosion range of the bomb
    [Tooltip("Number of segments used to display the explosion radius indicator")]
    public int segments = 360;
    public AudioClip explosionSfx;
    private float width = 0.25f;
    private LineRenderer line;
    private Rigidbody rb;
    private Vector3 throwForce;
    private bool throwNextUpdate = false;
    private bool hitGround = false;
    private Vector3 stayGrounded;

    [Networked] private float explosionRadius { get; set; }

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.startWidth = width;
        line.endWidth = width;
        Debug.Log(line);
        rb = GetComponent<Rigidbody>();
        Spawned();
    }

    private void Start()
    {
    }

    public void SetExplosionRadius(float radius)
    {
        this.explosionRadius = radius;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (hitGround)
        {
            transform.position = stayGrounded;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (throwNextUpdate)
        {
            rb.AddForce(throwForce, ForceMode.Impulse);
            throwNextUpdate = false;
        }
    }

    public void Throw(Vector3 throwForce)
    {
        this.throwForce = throwForce;
        throwNextUpdate = true;
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
                    // float adjustedExplosionForce = (explosionRadius / (2 * Mathf.Max(distance, 0.5f))) * explosionForce;
                    float adjustedExplosionForce = (explosionRadius / (explosionRadius * 0.5f * Mathf.Max(distance, 0.5f))) * explosionForce;
                    adjustedExplosionForce = Mathf.Clamp(adjustedExplosionForce, 1.5f, 2.25f);
                    Vector3 launchDirection = other.transform.position - impactPoint;
                    launchDirection.Normalize();
                    Debug.Log($"Adjusted explosion force: {adjustedExplosionForce}");
                    other.gameObject.GetComponent<NetworkCharacterMovementHandler>().AddForce(launchDirection, adjustedExplosionForce);
                    Debug.Log("Launching Player");
                }
            }
            AudioSource.PlayClipAtPoint(explosionSfx, gameObject.transform.position);
        }

        DespawnBomb();
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

    private void DespawnBomb()
    {
        if (NetworkPlayer.Local.Runner.IsServer)
            NetworkPlayer.Local.Runner.Despawn(this.GetComponent<NetworkObject>());
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GROUND") && !throwNextUpdate)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            hitGround = true;
            stayGrounded = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
            CreateExplosionRadiusIndicator();
        }
    }
}
