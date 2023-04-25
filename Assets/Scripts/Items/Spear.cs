using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{

    Rigidbody rb;
    Quaternion initialRotation;
    Vector3 initialForward;
    Vector3 rotateAxis;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // parentTransform = GetComponentInParent<Transform>();
        initialRotation = transform.rotation;
        initialForward = transform.forward;

        Vector3 xzForward = new Vector3(initialForward.x, 0f, initialForward.z);
        rotateAxis = Vector3.Cross(Vector3.up, xzForward);
        
        Debug.Log($"Rotate Axis before abs: {rotateAxis}");
        rotateAxis.Set(rotateAxis.x * -1, 0f, rotateAxis.z);
        Debug.Log($"Rotate Axis after abs: {rotateAxis}");
        

        // Debug.Log($"Spear Initial Forward: {initialForward}");
        // Debug.Log($"Spear Initial Rotation: {initialRotation}");
        // Debug.Log($"Spear Look Using initial forward: {Quaternion.LookRotation(initialForward)}");
        // parentTransform.rotation = Quaternion.LookRotation(initialForward);
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
            /*if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.z))
            {
                transform.Rotate(new Vector3(0f, 0f, 90f));
            } 
            else
            {
                transform.Rotate(new Vector3(90f, 0f, 0f));
            }*/
            // Vector3 lookRotation = new Vector3(initialForward.x, rb.velocity.y, initialForward.z);
            // Vector3 lookRotation = rb.velocity;
            // parentTransform.rotation = initialRotation * Quaternion.LookRotation(lookRotation.normalized);
            // Debug.Log($"Look Rotation: {parentTransform.rotation}");
            // Vector3 lookRotation = new Vector3(initialForward.x, rb.velocity.y, initialForward.z);
            // Vector3 velocityWithGravity = new Vector3(rb.velocity.x, -9.8f, rb.velocity.z);
            // lookRotation = Vector3.Scale(initialForward, rb.velocity);
            //Vector3 lookRotation = rb.velocity;
            // parentTransform.rotation = initialRotation * Quaternion.LookRotation(lookRotation);

            /*if (!rotateOnce)
            {
                rotateOnce = true;
                Vector3 lookRotation = new Vector3(initialForward.x, rb.velocity.y, initialForward.z);
                Vector3 velocityWithGravity = new Vector3(rb.velocity.x, -9.8f, rb.velocity.z);
                lookRotation = Vector3.Scale(initialForward, velocityWithGravity);
                //Vector3 lookRotation = rb.velocity;
                parentTransform.rotation = initialRotation * Quaternion.LookRotation(lookRotation);
                Debug.Log($"Look Rotation: {parentTransform.rotation}");
                Debug.Log($"Look Rotation Vector: {lookRotation}");
                Debug.Log($"Rigidbody Velocity: {rb.velocity}");
            }*/
        }
    }

    public void Launch(float launchVelocity, float launchAngleInRadians)
    {
        // float G = Physics.gravity.y;
        
        float zVel = launchVelocity * Mathf.Cos(launchAngleInRadians);
        float yVel = launchVelocity * Mathf.Sin(launchAngleInRadians);

        // float endXPos = initialPosition.x + xVel * timeInAir;
        // float endYPos = initialPosition.y + yVel * timeInAir + 0.5f * G * Mathf.Pow(timeInAir, 2);

        Vector3 localVelocity = new Vector3(0f, yVel, zVel);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // Debug.Log($"Spear local velocity: {localVelocity}");
        // Debug.Log($"Spear global velocity: {globalVelocity}");

        rb.velocity = globalVelocity;
        // rb.velocity = localVelocity;
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
