using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{

    Rigidbody rb;
    Quaternion initialRotation;
    Vector3 rotateAxis;
    CapsuleCollider capsuleCollider;
    bool hitGround = false;
    Vector3 stayGrounded;
    public AudioClip hitSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        initialRotation = transform.rotation;

        Vector3 initialForward = transform.forward;
        Vector3 xzForward = new Vector3(initialForward.x, 0f, initialForward.z);
        rotateAxis = Vector3.Cross(Vector3.up, xzForward);
        rotateAxis.Set(rotateAxis.x * -1, 0f, rotateAxis.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitGround)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity) * initialRotation;
            transform.Rotate(rotateAxis, 90f);
            Debug.Log("Rotated");
        }
        else
        {
            transform.position = stayGrounded;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GROUND"))
        {
            stayGrounded = new Vector3(transform.position.x, Mathf.Max(transform.position.y, GetComponent<MeshRenderer>().bounds.extents.y), transform.position.z);
            rb.velocity = Vector3.zero;
            rb.freezeRotation = true;
            rb.useGravity = false;
            capsuleCollider.isTrigger = false;
            hitGround = true;
            //AudioSource.PlayClipAtPoint(hitSound, other.transform.position);
        }
    }
}
