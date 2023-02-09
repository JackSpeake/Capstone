using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    private Transform startingPos;
    [SerializeField] private float hoverRotate = 1f;
    [SerializeField] private float hoverHeight = .1f;
    [SerializeField] private float hoverSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // not working
        transform.position = 
            startingPos.position + 
            new Vector3(0, 
            Mathf.Lerp(hoverHeight * -1, hoverHeight, hoverSpeed * Time.deltaTime), 
            0);
    }
}
