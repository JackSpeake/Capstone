using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class NetworkCharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
    Vector2 viewinput;
    Camera localCamera;

    //rotation
    float cameraRotationX = 0;
    
    private void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
        localCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        cameraRotationX += viewinput.y * Time.deltaTime * networkCharacterControllerPrototype.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            // rotate
            networkCharacterControllerPrototype.Rotate(networkInputData.rotationInput);

            // move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototype.Move(moveDirection);

            // jump
            if (networkInputData.jumpPressed)
                networkCharacterControllerPrototype.Jump();
        }
    }

    public void SetViewInputVector(Vector2 viewInputVector)
    {
        viewinput = viewInputVector;
    }
}
