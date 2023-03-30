using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{

    Rigidbody rb;
    Quaternion initialRotation;
    Vector3 initialForward;
    Transform parentTransform;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        parentTransform = GetComponentInParent<Transform>();
        initialRotation = parentTransform.rotation;
        initialForward = parentTransform.forward;
        Debug.Log($"Spear Initial Forward: {initialForward}");
        Debug.Log(GetComponent<MeshRenderer>().bounds.extents.y);
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
            //Vector3 lookRotation = new Vector3(initialForward.x, rb.velocity.y, initialForward.z);
            parentTransform.rotation = initialRotation * Quaternion.LookRotation(rb.velocity);
            Debug.Log($"Look Rotation: {transform.rotation}");
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
