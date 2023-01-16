using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool jump = false;

    NetworkCharacterMovementHandler networkCharacterMovementHandler;

    private void Awake()
    {
        networkCharacterMovementHandler = GetComponent<NetworkCharacterMovementHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // view input

        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        networkCharacterMovementHandler.SetViewInputVector(viewInputVector);

        // move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        jump = Input.GetButtonDown("Jump");
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.rotationInput = viewInputVector.x;
        networkInputData.movementInput = moveInputVector;
        networkInputData.jumpPressed = jump;

        return networkInputData;
    }
}
