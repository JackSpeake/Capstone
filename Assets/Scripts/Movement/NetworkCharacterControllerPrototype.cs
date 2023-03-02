using System;
using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class NetworkCharacterControllerPrototype : NetworkTransform
{
    [Header("Character Controller Settings")]
    [Tooltip("The rate at which people fall")]
    public float gravity = -20.0f;
    [Tooltip("The initial magnitude of a jump")]
    public float jumpImpulse = 8.0f;
    private float acceleration = 10.0f; // current value
    [Tooltip("The rate the player accelerates in the air")]
    public float airAcceleration = 1.0f;
    [Tooltip("The rate the player accelerates on the ground")]
    public float groundAcceleration = 10.0f;
    private float braking = 10.0f; // current value
    [Tooltip("The rate of breaking in the air")]
    public float Airbraking = 0.3f;
    [Tooltip("The rate of breaking on the ground")]
    public float Groundbraking = 10.0f;
    private float maxSpeed = 2.0f;
    [Tooltip("The maximum speed in the air")]
    public float maxSpeedAir = 100.0f;
    [Tooltip("The maximum speed on the ground")]
    public float maxSpeedG = 2.0f;
    [Tooltip("The multiplyer for the next tick of movement")]
    public float VelMult = 1.0f;
    [Tooltip("The speed at which the player rotates left and right")]
    public float rotationSpeed = 15.0f;
    [Tooltip("The speed at which the player rotates up and down")]
    public float viewUpDownRotationSpeed = 50.0f;
    [Tooltip("The speed at which the max speed changes between the ground and the air")]
    public float maxSpeedLerp = 10.0f;
    [Tooltip("The strength of strafing speed loss")]
    public float strafeSpeed = 1.0f;
    [Tooltip("The amount of air speed that can be preserved by bunny hopping")]
    public float bunnyHopSpeedMax = 2.0f;
    [Tooltip("The base velocity dashes are applied to")]
    public float standStillShiftSpeed = 20.0f;
    [Tooltip("The speed at which the player moves when sliding")]
    public float slideWarp = .1f;

    [Networked]
    [HideInInspector]
    public bool IsGrounded { get; set; }

    [Networked]
    [HideInInspector]
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Sets the default teleport interpolation velocity to be the CC's current velocity.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

    /// <summary>
    /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
    /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
    /// </summary>
    protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

    public CharacterController Controller { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CacheController();
    }

    public override void Spawned()
    {
        base.Spawned();
        CacheController();
    }

    private void CacheController()
    {
        if (Controller == null)
        {
            Controller = GetComponent<CharacterController>();

            Assert.Check(Controller != null, $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
        }
    }

    protected override void CopyFromBufferToEngine()
    {
        // Trick: CC must be disabled before resetting the transform state
        Controller.enabled = false;

        // Pull base (NetworkTransform) state from networked data buffer
        base.CopyFromBufferToEngine();

        // Re-enable CC
        Controller.enabled = true;
    }

    /// <summary>
    /// Basic implementation of a jump impulse (immediately integrates a vertical component to Velocity).
    /// <param name="ignoreGrounded">Jump even if not in a grounded state.</param>
    /// <param name="overrideImpulse">Optional field to override the jump impulse. If null, <see cref="jumpImpulse"/> is used.</param>
    /// </summary>
    public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
    {
        if (IsGrounded || ignoreGrounded)
        {
            var newVel = Velocity;
            newVel.y += overrideImpulse ?? jumpImpulse;
            Velocity = newVel;
        }
    }

    public void ShiftDirection(Vector3 direction)
    {
        if (Velocity.magnitude > 10)
        {
            Velocity = direction.normalized * Velocity.magnitude * VelMult;
            VelMult = 1.0f;
        }
        else
        {
            Velocity = direction.normalized * standStillShiftSpeed * VelMult;
            VelMult = 1.0f;
        }
    }

    /// <summary>
    /// Basic implementation of a character controller's movement function based on an intended direction.
    /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
    /// </summary>
    public virtual void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction = direction.normalized;

        if (IsGrounded)
        {
            acceleration = groundAcceleration;
            braking = Groundbraking;
            maxSpeed = Mathf.Lerp(maxSpeed, maxSpeedG, Time.deltaTime * maxSpeedLerp);
        }
        if (!IsGrounded)
        {
            acceleration = airAcceleration;
            braking = Airbraking;
            maxSpeed = Mathf.Lerp(maxSpeed, maxSpeedAir, Time.deltaTime * maxSpeedLerp);
        }

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;

        horizontalVel.z = moveVelocity.z;
        horizontalVel *= VelMult;
        VelMult = 1.0f;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
        }
        else if (IsGrounded)
        {
            // Can cause unstrafable slide
            var newSpeed = direction * acceleration * deltaTime;
            if (horizontalVel.magnitude > maxSpeedG + bunnyHopSpeedMax)
            {
                // Project the new speed direction onto the right/left vector

                newSpeed = newSpeed * slideWarp;
            }
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + newSpeed, maxSpeed);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);
        }
        else
        {
            var mag = horizontalVel.magnitude;
            var originalDir = horizontalVel.normalized;
            horizontalVel = Vector3.ClampMagnitude(Vector3.Lerp(originalDir, direction, deltaTime * strafeSpeed) * mag, maxSpeed);
        }

        if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }

        moveVelocity.y += gravity * Runner.DeltaTime;

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        Controller.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        IsGrounded = Controller.isGrounded;
    }

    public void Rotate(float rotationY)
    {
        transform.Rotate(0, rotationY * Runner.DeltaTime * rotationSpeed, 0);
    }

    public void AddForce(Vector3 direction, float force)
    {
        StartCoroutine(LerpMove(direction, force));
    }

    private IEnumerator LerpMove(Vector3 direction, float force)
    {
        float forceEachFrame = force / 10;
        for (int i = 0; i < 10; i++)
        {
            Controller.Move(direction * forceEachFrame);
            yield return new WaitForFixedUpdate();
        }
    }
}