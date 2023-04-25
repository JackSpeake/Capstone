using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{

    Rigidbody rb;
    Quaternion initialRotation;
    Vector3 rotateAxis;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;

        Vector3 initialForward = transform.forward;
        Vector3 xzForward = new Vector3(initialForward.x, 0f, initialForward.z);
        rotateAxis = Vector3.Cross(Vector3.up, xzForward);
        rotateAxis.Set(rotateAxis.x * -1, 0f, rotateAxis.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= GetComponent<MeshRenderer>().bounds.extents.y)
        {
            Vector3 stayGrounded = new Vector3(transform.position.x, Mathf.Max(transform.position.y, 0f), transform.position.z);
            transform.position = stayGrounded;
            rb.freezeRotation = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity) * initialRotation;
            transform.Rotate(rotateAxis, 90f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GROUND"))
        {
            rb.velocity = Vector3.zero;
            rb.freezeRotation = true;
        }
    }
}
