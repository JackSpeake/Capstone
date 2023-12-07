using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Goo : NetworkBehaviour
{
    [SerializeField] public float SlowRatio = .5f;
    [SerializeField] public float SlipSpeedRatio = 1.5f;
    private bool throwFirstUpdate = false;
    private Rigidbody rb;
    private Vector3 throwForce;
    private bool fast;
    [ColorUsage(true, true)]
    [SerializeField] public Color slowColor, fastColor;
    [SerializeField] public float gooTimer = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        throwFirstUpdate = true;
        Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        if (throwFirstUpdate)
        {
            rb.AddForce(throwForce, ForceMode.Impulse);
            throwFirstUpdate = false;
        }
    }

    public void Setup(bool fast, Vector3 tf)
    {
        this.fast = fast;

        if (fast)
        {
            // CHANGE COLOR
        }

        this.throwForce = tf;
    }

    private void HitPlatform(GameObject ground)
    {
        GooablePlatform gp = ground.GetComponent<GooablePlatform>();
        if (fast)
        {
            gp.HitWithGoo(SlipSpeedRatio, gooTimer, fastColor);
        }
        else
        {
            gp.HitWithGoo(SlowRatio, gooTimer, slowColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("GROUND") || other.gameObject.CompareTag("WALL")) 
            && !throwFirstUpdate)
        {
            HitPlatform(other.gameObject);
            DespawnGoo();
        }
    }

    private void DespawnGoo()
    {
        if (NetworkPlayer.Local.Runner.IsServer)
            NetworkPlayer.Local.Runner.Despawn(this.GetComponent<NetworkObject>());
        Destroy(gameObject);
    }
}
