using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator anim;
    float speed = 0.0f;
    float accel = 0.5f;
    float decel = 0.5f;
    float maxSpeed = 1f;
    float minSpeed = 0f;
    int velHash;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log("Animator Linked");
        velHash = Animator.StringToHash("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        // key input variables
        bool forwardPress = Input.GetKey("w");

        if (forwardPress && speed < maxSpeed)
        {
            speed += Time.deltaTime * accel;
        } 
        else if (!forwardPress && speed > minSpeed)
        {
            speed -= Time.deltaTime * decel;
        } 
        else if (speed < minSpeed)
        {
            speed = 0f;
        }

        anim.SetFloat(velHash, speed);
    }
}
