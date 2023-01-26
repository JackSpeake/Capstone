using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class NetworkCharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    Camera localCamera;
    bool wallSliding = false;
    NetworkInputData networkInputData;
    Vector3 lockedWallVelocity;
    [SerializeField] private float slideslow = 3;
    [SerializeField] private float wallSlideGravity; //Set to gravity value of character controller / slideslow by default

    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        localCamera = GetComponentInChildren<Camera>();
        wallSlideGravity = networkCharacterControllerPrototype.gravity / slideslow;
    }

    public override void FixedUpdateNetwork()
    {
        if (networkCharacterControllerPrototype.IsGrounded && wallSliding)
        {
            wallSliding = false;
            networkCharacterControllerPrototype.gravity *= slideslow;
        }
        if (GetInput(out networkInputData))
        {
            // rotate
            transform.forward = networkInputData.aimForwardVector;

            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);
            transform.rotation = rot;

            if (wallSliding && !networkInputData.jumpHeld)
            {
                wallSliding = false;
                networkCharacterControllerPrototype.Jump(ignoreGrounded: true);
                networkCharacterControllerPrototype.gravity *= slideslow;
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
            lockedWallVelocity = Vector3.Project(gameObject.transform.forward, hit.gameObject.transform.right);
        }
    }
}
