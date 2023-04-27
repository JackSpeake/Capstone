using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstep;
    public AudioClip dash;
    bool grounded = true;
    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;

    public void Awake()
    {
        networkCharacterControllerPrototype = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public void Update()
    {
        //Debug.Log(networkCharacterControllerPrototype.IsGrounded);
        if (networkCharacterControllerPrototype.IsGrounded && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) 
        {
            footstep.enabled = true;
        }
        else 
        {
            footstep.enabled = false;
        }

        if (!networkCharacterControllerPrototype.IsGrounded && (Input.GetKeyDown(KeyCode.LeftShift))) {
            AudioSource.PlayClipAtPoint(dash, gameObject.transform.position);
        }
    }
}
