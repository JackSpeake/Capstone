using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Photon;

enum PlayerState
{
    Running,
    InAir,
    WallSliding,
    WallJumping,
    AirDashing
}

public class NetworkCharacterMovementHandler : NetworkBehaviour
{
    PlayerState state = PlayerState.Running;
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    Camera localCamera;
    bool wallSliding = false;
    bool dashed = false;
    NetworkInputData networkInputData;
    [Tooltip("The starting val for multiplying speed")]
    [SerializeField] float basevel = 1.0f;
    [Tooltip("The rate at which speed is reduced actually idk what this does")]
    [SerializeField] float speedReductionRate = 1.0f;
    Vector3 lockedWallVelocity;
    Vector3 lockedWallDirection;
    [Tooltip("The multiplier for speed gains when jumping off the wall")]
    [SerializeField] float wallJumpEjectMultiplier = 1.25f;
    Vector3 moveDirection;
    [Tooltip("The amount of speed gained upon dashing")]
    [SerializeField] float dashSpeed = 1.5f;
    [Tooltip("The slowdown when sliding")]
    [SerializeField] private float slideslow = 3;
    [Tooltip("Set to gravity value of character controller / slideslow by default")]
    [SerializeField] private float wallSlideGravity;
    [Tooltip("Set to gravity value of character controller by default")]
    [SerializeField] private float regularGraviity; 
    [Tooltip("Time that the player is jumping away from the wall for")]
    [SerializeField] private float wallJumpTime = 0.2f; 
    private Vector3 wallJumpDirection;
    private float wallJumpTimer; // Keeps track of how long it's been since the player jumped off the wall

    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        localCamera = GetComponentInChildren<Camera>();
        regularGraviity = networkCharacterControllerPrototype.gravity;
        wallSlideGravity = networkCharacterControllerPrototype.gravity / slideslow;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out networkInputData))
        {
            RotateCharacter();
            // calculate move direction
            moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            if (networkCharacterControllerPrototype.IsGrounded)
            {
                dashed = false;
                state = PlayerState.Running;
                networkCharacterControllerPrototype.gravity = regularGraviity;
            }
            switch (state)
            {
                case PlayerState.Running:
                    networkCharacterControllerPrototype.VelMult =
                        Mathf.Lerp(networkCharacterControllerPrototype.VelMult, basevel, Time.deltaTime * speedReductionRate);
                    if (networkInputData.jumpPressed)
                        networkCharacterControllerPrototype.Jump();
                    state = PlayerState.InAir;
                    break;
                case PlayerState.InAir:
                    if (!dashed && networkInputData.dashPressed)
                    {
                        dashed = true;
                        networkCharacterControllerPrototype.VelMult *= dashSpeed;
                        networkCharacterControllerPrototype.ShiftDirection(Camera.main.transform.forward);
                        
                    }
                    break;
                case PlayerState.WallSliding:
                    moveDirection = lockedWallDirection;
                    // Keep the x and z velocities the same but allow the y velocity to change (gravity eneds to work lol)
                    lockedWallVelocity.y = networkCharacterControllerPrototype.Velocity.y;
                    networkCharacterControllerPrototype.Velocity = lockedWallVelocity;
                    Debug.Log($"Move Direction while wall sliding: {moveDirection}");
                    Debug.Log($"Velocity while wall sliding: {networkCharacterControllerPrototype.Velocity}");
                    if (networkCharacterControllerPrototype.Velocity.y < 0) //Gravity only changed when falling so the player doesn't jump higher than intended
                    {
                        networkCharacterControllerPrototype.gravity = wallSlideGravity;
                    }

                    if (!networkInputData.jumpHeld)
                    {
                        // wallSliding = false;
                        state = PlayerState.WallJumping;
                        networkCharacterControllerPrototype.gravity = regularGraviity;
                        wallJumpTimer = wallJumpTime;
                        wallJumpDirection = Camera.main.transform.forward;
                        //networkCharacterControllerPrototype.Jump(ignoreGrounded: true);
                        if (networkCharacterControllerPrototype.VelMult == 0)
                            networkCharacterControllerPrototype.VelMult = 1;
                        networkCharacterControllerPrototype.VelMult *= wallJumpEjectMultiplier;
                        networkCharacterControllerPrototype.ShiftDirection(wallJumpDirection);
                        //moveDirection = hitWallNormal * wallJumpEjectMultiplier;
                    }
                    break;
                case PlayerState.WallJumping:
                    if (wallJumpTimer > 0)
                    {
                        moveDirection = wallJumpDirection;
                        //moveDirection = hitWallNormal * wallJumpEjectMultiplier;
                        wallJumpTimer -= Time.deltaTime;
                    }
                    else
                    {
                        state = PlayerState.InAir;
                    }
                    break;
                case PlayerState.AirDashing:
                    // I have no idea if anything belongs here based on the old code because it doesn't seem like anything special happens
                    break;
                default:
                    Debug.Log("Something went very wrong here because the player is not in a proper state");
                    break;
            }
            networkCharacterControllerPrototype.Move(moveDirection);
        }

        /*
        if (networkCharacterControllerPrototype.IsGrounded && wallSliding)
        {
            wallSliding = false;
            networkCharacterControllerPrototype.gravity = regularGraviity;
        }

        if (GetInput(out networkInputData))
        {
            if (networkCharacterControllerPrototype.IsGrounded)
                dashed = false;

            if (!wallSliding && !dashed && !networkCharacterControllerPrototype.IsGrounded && networkInputData.dashPressed)
            {
                dashed = true;
                networkCharacterControllerPrototype.VelMult *= dashSpeed;
            }

            // move
            moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();


            if (wallSliding && !networkInputData.jumpHeld)
            {
                wallSliding = false;
                networkCharacterControllerPrototype.Jump(ignoreGrounded: true);
                networkCharacterControllerPrototype.gravity = regularGraviity;
                moveDirection = hitWallNormal * wallJumpEjectMultiplier;
            }

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
        }*/
    }

    private void RotateCharacter()
    {
        // rotate
        transform.forward = networkInputData.aimForwardVector;

        Quaternion rot = transform.rotation;
        rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);
        transform.rotation = rot;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("WALL") && networkInputData.jumpPressed && !networkCharacterControllerPrototype.IsGrounded)
        {
            Debug.Log("Wall Sliding");
            // wallSliding = true;
            state = PlayerState.WallSliding;
            dashed = false;
            lockedWallVelocity = networkCharacterControllerPrototype.Velocity;
            lockedWallDirection = Vector3.Project(moveDirection, hit.gameObject.transform.right);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RESETCUBE") && dashed)
        {
            dashed = false;
            StartCoroutine(DisableCube(other.gameObject));
            Debug.Log("DASH RESET");
        }
    }


    private IEnumerator DisableCube(GameObject cube)
    {
        cube.SetActive(false);
        yield return new WaitForSeconds(3f);
        cube.SetActive(true);
    }

    private IEnumerator AirDash()
    {

        networkCharacterControllerPrototype.gravity = .01f;

        yield return new WaitForSeconds(.5f);

        networkCharacterControllerPrototype.gravity = regularGraviity;
    }
}
