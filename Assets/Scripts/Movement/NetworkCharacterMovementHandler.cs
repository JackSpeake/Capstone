using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;
using Photon;

enum PlayerState
{
    Running,
    InAir,
    WallSliding,
    WallJumping,
    AirDashing,
    Stumble
}

public class NetworkCharacterMovementHandler : NetworkBehaviour
{
    PlayerState state = PlayerState.Running;
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    [SerializeField] Camera localCamera;
    bool wallSliding = false;
    bool dashed = false;
    NetworkObject networkObject;
    NetworkInputData networkInputData;
    [Tooltip("The starting val for multiplying speed")]
    [SerializeField] float basevel = 1.0f;
    [Tooltip("The rate at which speed is reduced actually idk what this does")]
    [SerializeField] float speedReductionRate = 1.0f;
    Vector3 lockedWallVelocity;
    Vector3 lockedWallDirection;
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
    [Header("Wall Jump Thresholds")]
    [Tooltip("Vignette color for wall sliding before reaching a threshold (think Mario Kart drift colors)")]
    [SerializeField] private Color wallSlideNoBoostColor;
    [Tooltip("Number of seconds wallsliding to reach the first threshold")]
    [SerializeField] private float wallJumpLowThreshold = 0.5f;
    [Tooltip("Velocity Multiplier when jumping off the wall for the first threshold")]
    [SerializeField] private float wallJumpLowBoostMultiplier = 1.1f;
    [Tooltip("Vignette color for the first threshold (think Mario Kart drift colors)")]
    [SerializeField] private Color wallJumpLowBoostColor;
    [Space(5)]
    [Tooltip("Number of seconds wallsliding to reach the second threshold")]
    [SerializeField] private float wallJumpMediumThreshold = 1.0f;
    [Tooltip("Velocity Multiplier when jumping off the wall for the second threshold")]
    [SerializeField] private float wallJumpMediumBoostMultiplier = 1.25f;
    [Tooltip("Vignette color for the second threshold (think Mario Kart drift colors)")]
    [SerializeField] private Color wallJumpMediumBoostColor;
    [Space(5)]
    [Tooltip("Number of seconds wallsliding to reach the third (last) threshold")]
    [SerializeField] private float wallJumpBigThreshold = 1.5f;
    [Tooltip("Velocity Multiplier when jumping off the wall for the third (last) threshold")]
    [SerializeField] private float wallJumpBigBoostMultiplier = 1.4f;
    [Tooltip("Vignette color for the third (last) threshold (think Mario Kart drift colors)")]
    [SerializeField] private Color wallJumpBigBoostColor;
    [Tooltip("The crosshair image attached to the player")]
    [SerializeField] private Image crosshair;
    [Tooltip("The speedometer attached to the player")]
    [SerializeField] private Slider speedometer;
    private Color initialCrosshairColor;
    private Vector3 wallJumpDirection;
    private float wallJumpTimer; // Keeps track of how long it's been since the player jumped off the wall
    private float wallSlideTimer = 0.0f; //Keeps track of how long a player has been wall sliding for
    private PostProcessingEffectsController postProcessingEffectsController;
    [SerializeField] private Vector3 TrueVal;
    private bool teleportOnNextTick;
    private Vector3 teleportPosition;

    public int checkpoint = 0;
    public AudioSource wallslideSfx;

    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        postProcessingEffectsController = GetComponentInChildren<PostProcessingEffectsController>();
        localCamera = GetComponentInChildren<Camera>();
        regularGraviity = networkCharacterControllerPrototype.gravity;
        wallSlideGravity = networkCharacterControllerPrototype.gravity / slideslow;
        initialCrosshairColor = crosshair.color;
    }

    private void Start()
    {
        networkObject = this.GetComponent<NetworkObject>();
    }

    public void DashResetFunc()
    {
        dashed = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (teleportOnNextTick)
        {
            teleportOnNextTick = false;
            networkCharacterControllerPrototype.TeleportToPosition(teleportPosition);
        }
        else if (GetInput(out networkInputData))
        {
            RotateCharacter();
            // calculate move direction
            moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            speedometer.value = networkCharacterControllerPrototype.Velocity.magnitude * 2f;
            if (networkCharacterControllerPrototype.IsGrounded)
            {
                dashed = false;
                state = PlayerState.Running;
                postProcessingEffectsController.SetVignetteActive(0.0f);
                crosshair.color = initialCrosshairColor;
                networkCharacterControllerPrototype.gravity = regularGraviity;
            }
            switch (state)
            {
                case PlayerState.Running:
                    wallslideSfx.enabled = false;
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
                        
                        networkCharacterControllerPrototype.ShiftDirection(TrueVal);
                        
                       

                        
                    }
                    break;
                case PlayerState.WallSliding:
                    wallslideSfx.enabled = true;
                    postProcessingEffectsController.SetVignetteActive(1.0f);
                    wallSlideTimer += Time.fixedDeltaTime;
                    Debug.Log(wallSlideTimer);
                    moveDirection = lockedWallDirection;
                    // Keep the x and z velocities the same but allow the y velocity to change (gravity eneds to work lol)
                    lockedWallVelocity.y = networkCharacterControllerPrototype.Velocity.y;
                    networkCharacterControllerPrototype.Velocity = lockedWallVelocity;
                    float wallJumpEjectMultiplier;
                    if (wallSlideTimer >= wallJumpBigThreshold)
                    {
                        Debug.Log("Big Boost");
                        wallJumpEjectMultiplier = wallJumpBigBoostMultiplier;
                        postProcessingEffectsController.SetColor(wallJumpBigBoostColor);
                        crosshair.color = wallJumpBigBoostColor;

                    }
                    else if (wallSlideTimer >= wallJumpMediumThreshold)
                    {
                        Debug.Log("Medium Boost");
                        wallJumpEjectMultiplier = wallJumpMediumBoostMultiplier;
                        postProcessingEffectsController.SetColor(wallJumpMediumBoostColor);
                        crosshair.color = wallJumpMediumBoostColor;
                    }
                    else if (wallSlideTimer >= wallJumpLowThreshold)
                    {
                        Debug.Log("Low Boost");
                        wallJumpEjectMultiplier = wallJumpLowBoostMultiplier;
                        postProcessingEffectsController.SetColor(wallJumpLowBoostColor);
                        crosshair.color = wallJumpLowBoostColor;
                    }
                    else
                    {
                        Debug.Log("No Boost");
                        wallJumpEjectMultiplier = 1.0f;
                        postProcessingEffectsController.SetColor(wallSlideNoBoostColor);
                        crosshair.color = wallSlideNoBoostColor;
                    }
                    //Debug.Log($"Move Direction while wall sliding: {moveDirection}");
                    //Debug.Log($"Velocity while wall sliding: {networkCharacterControllerPrototype.Velocity}");
                    if (networkCharacterControllerPrototype.Velocity.y < 0) //Gravity only changed when falling so the player doesn't jump higher than intended
                    {
                        networkCharacterControllerPrototype.gravity = wallSlideGravity;
                    }

                    if (!networkInputData.jumpHeld)
                    {
                        // wallSliding = false;
                        postProcessingEffectsController.SetVignetteActive(0.0f);
                        crosshair.color = initialCrosshairColor;
                        state = PlayerState.WallJumping;
                        networkCharacterControllerPrototype.gravity = regularGraviity;
                        wallJumpTimer = wallJumpTime;
                        wallJumpDirection = TrueVal;
                        //networkCharacterControllerPrototype.Jump(ignoreGrounded: true);
                        if (networkCharacterControllerPrototype.VelMult == 0)
                            networkCharacterControllerPrototype.VelMult = 1;
                        
                        networkCharacterControllerPrototype.VelMult *= wallJumpEjectMultiplier;
                        networkCharacterControllerPrototype.ShiftDirection(wallJumpDirection);
                        //moveDirection = hitWallNormal * wallJumpEjectMultiplier;
                    }
                    break;
                case PlayerState.WallJumping:
                    wallslideSfx.enabled = false;
                    if (wallJumpTimer > 0)
                    {
                        moveDirection = wallJumpDirection;
                        //moveDirection = hitWallNormal * wallJumpEjectMultiplier;
                        wallJumpTimer -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        state = PlayerState.InAir;
                    }
                    break;
                case PlayerState.AirDashing:
                    wallslideSfx.enabled = false;
                    // I have no idea if anything belongs here based on the old code because it doesn't seem like anything special happens
                    break;
                case PlayerState.Stumble:
                    wallslideSfx.enabled = false;
                    AddForce(Vector3.down, 1.0f);
                    break;
                default:
                    wallslideSfx.enabled = false;
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

    public void TeleportToPosition(Vector3 position)
    {
        teleportOnNextTick = true;
        teleportPosition = position;
    }

    public void AddForce(Vector3 direction, float force)
    {
        networkCharacterControllerPrototype.VelMult *= force;
        networkCharacterControllerPrototype.ShiftDirection(direction);
    }

    private void RotateCharacter()
    {
        // rotate
        transform.forward = networkInputData.aimForwardVector;

        Quaternion rot = transform.rotation;
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);
        transform.rotation = rot;
        TrueVal = transform.forward;
        rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);
        transform.rotation = rot;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("WALL") && networkInputData.jumpHeld && !networkCharacterControllerPrototype.IsGrounded && state != PlayerState.WallSliding && state != PlayerState.WallJumping)
        {
            Debug.Log("Wall Sliding");
            // wallSliding = true;
            state = PlayerState.WallSliding;
            dashed = false;
            lockedWallVelocity = networkCharacterControllerPrototype.Velocity;
            lockedWallDirection = Vector3.Project(moveDirection, hit.gameObject.transform.right);
            wallSlideTimer = 0.0f;
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
        if (other.gameObject.CompareTag("HURT"))
        {
            Debug.Log("I got hurt");
            state = PlayerState.Stumble;
            dashed = false;
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
