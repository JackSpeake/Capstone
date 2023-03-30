using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateSpeed = .1f;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Translate(Vector3.right * Time.deltaTime * rotateSpeed);
        transform.Rotate(offset);
    }

    public static void quit()
    {
        Application.Quit();
    }
}
