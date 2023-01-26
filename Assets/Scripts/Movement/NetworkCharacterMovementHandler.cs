using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Photon;

public class NetworkCharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    Camera localCamera;
    bool wallSliding = false;
    bool dashed = false;
    NetworkInputData networkInputData;
    [SerializeField] float basevel = 1.0f;
    [SerializeField] float speedReductionRate = 1.0f;
    Vector3 lockedWallVelocity;
    [SerializeField] float dashSpeed = 1.5f;
    [SerializeField] private float slideslow = 3;
    [SerializeField] private float wallSlideGravity; //Set to gravity value of character controller / slideslow by default
    [SerializeField] private float regularGraviity; // Set to gravity value of character controller by default

    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        localCamera = GetComponentInChildren<Camera>();
        regularGraviity = networkCharacterControllerPrototype.gravity;
        wallSlideGravity = networkCharacterControllerPrototype.gravity / slideslow;
    }

    public override void FixedUpdateNetwork()
    {
        if (networkCharacterControllerPrototype.IsGrounded && wallSliding)
        {
            wallSliding = false;
            networkCharacterControllerPrototype.gravity = regularGraviity;
        }
        if (GetInput(out networkInputData))
        {
            // rotate
            transform.forward = networkInputData.aimForwardVector;

            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);
            transform.rotation = rot;

            if (networkCharacterControllerPrototype.IsGrounded)
                dashed = false;

            if (!wallSliding && !dashed && !networkCharacterControllerPrototype.IsGrounded && networkInputData.dashPressed)
            {
                dashed = true;
                networkCharacterControllerPrototype.VelMult *= dashSpeed;
            }

            if (wallSliding && !networkInputData.jumpHeld)
            {
                wallSliding = false;
                networkCharacterControllerPrototype.Jump(ignoreGrounded: true);
                networkCharacterControllerPrototype.gravity = regularGraviity;
            }

            // move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            if (wallSliding)
            {
                moveDirection = lockedWallVelocity;
                if (networkCharacterControllerPrototype.Velocity.y < 0) //Gravity only changed when falling so the player doesn't jump higher than intended
                {
                    networkCharacterControllerPrototype.gravity = wallSlideGravity;
                }
            }

            if (networkCharacterControllerPrototype.IsGrounded)
            {
                networkCharacterControllerPrototype.VelMult = 
                    Mathf.Lerp(networkCharacterControllerPrototype.VelMult, basevel, Time.deltaTime * speedReductionRate);
            }

            networkCharacterControllerPrototype.Move(moveDirection);

            // jump
            if (networkInputData.jumpPressed)
                networkCharacterControllerPrototype.Jump();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("WALL") && networkInputData.jumpPressed && !networkCharacterControllerPrototype.IsGrounded)
        {
            Debug.Log("Wall Sliding");
            wallSliding = true;
            dashed = false;
            lockedWallVelocity = Vector3.Project(hit.gameObject.transform.forward, hit.gameObject.transform.right);
        }
    }

    private IEnumerator AirDash()
    {

        networkCharacterControllerPrototype.gravity = .01f;

        yield return new WaitForSeconds(.5f);

        networkCharacterControllerPrototype.gravity = regularGraviity;
    }
}
