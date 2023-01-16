using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharControlBasic : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float slideslow = 3;
    private float currGravity;
    private bool sprinting = false;
    private bool wallsliding = true;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        currGravity = gravityValue;
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        sprinting = Input.GetKey(KeyCode.LeftShift);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (sprinting) move *= 2;
        move = gameObject.transform.rotation * move * Time.deltaTime * playerSpeed;
        move.y = 0;
        controller.Move( move);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * currGravity);
        }

        
       
        playerVelocity.y += currGravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("WALL") && Input.GetButton("Jump"))
        {
            Debug.Log("TEST");
            currGravity /= slideslow;
        }
    }
}