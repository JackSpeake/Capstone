using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    private Vector3 startingPos;
    private float startTime;
    [SerializeField] private float hoverRotate = 1f;
    [SerializeField] private float hoverHeight = .1f;
    [SerializeField] private float hoverSpeed = 1;
    [SerializeField] private Material nonGlowMaterial, glowMaterial;
    private MeshRenderer meshRenderer;
    private float offset;
    bool pause = false;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        startingPos = transform.position;
        hoverSpeed = hoverSpeed + Random.Range(-.1f, .1f);
        hoverHeight = hoverHeight + Random.Range(-.1f, .1f);
        offset = Random.Range(-1.5f, 1.5f);

        if (!GetComponent<GooablePlatform>())
            this.gameObject.AddComponent<GooablePlatform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!pause)
        {
            transform.position = 
                startingPos + 
                new Vector3(0,
                Mathf.PingPong(hoverSpeed * (Time.time + offset), hoverHeight),
                0);
            transform.Rotate(new Vector3(0, hoverRotate, 0));
        }
    }

    public void PlayerCollided()
    {
        
        if (GetComponent<GooablePlatform>() && GetComponent<GooablePlatform>().gooed)
        {
            GetComponent<GooablePlatform>().LandedPlatform();
        }
        else
        {
            meshRenderer.material = glowMaterial;
            pause = true;
        }
        
    }


    public void PlayerCollideStop()
    {
        if (GetComponent<GooablePlatform>() && GetComponent<GooablePlatform>().gooed)
        {
            GetComponent<GooablePlatform>().LeftPlatform();
        }
        else
        {
            pause = false;
            meshRenderer.material = nonGlowMaterial;
        }
    }
}
