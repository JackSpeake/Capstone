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
    private float offset;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        hoverSpeed = hoverSpeed + Random.Range(-.1f, .1f);
        hoverHeight = hoverHeight + Random.Range(-.1f, .1f);
        offset = Random.Range(-1.5f, 1.5f);

    }

    // Update is called once per frame
    void Update()
    {
        // not working
        transform.position = 
            startingPos + 
            new Vector3(0,
            Mathf.PingPong(hoverSpeed * (Time.time + offset), hoverHeight),
            0);
        transform.Rotate(new Vector3(0, hoverRotate, 0));
    }
}
