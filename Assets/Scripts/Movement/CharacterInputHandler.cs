using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool jump = false;
    bool jumpHeld = false;
    bool shift = false;
    bool ePressed = false;
    bool qPressed = false;

    LocalCameraHandler localCameraHandler;
    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
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

        // move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButton("Jump"))
        {
            jumpHeld = true; // Separate check to see if player is holding down space to maintain a wall slide
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            shift = true;
        }

        if (Input.GetKey(KeyCode.E))
        {
            ePressed = true;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            qPressed = true;
        }


        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;
        networkInputData.movementInput = moveInputVector;
        networkInputData.jumpPressed = jump;
        networkInputData.jumpHeld = jumpHeld;
        networkInputData.dashPressed = shift;
        networkInputData.selfItemPressed = ePressed;
        networkInputData.throwItemPressed = qPressed;

        shift = false;
        ePressed = false;
        qPressed = false;

        jump = false;
        jumpHeld = false;

        return networkInputData;
    }
}
