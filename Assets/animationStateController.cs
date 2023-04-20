using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator anim;
    float speed = 0.0f;
    float accel = 1f;
    float decel = 1f;
    float maxSpeed = 1f;
    float minSpeed = 0f;
    float halfSpeed = 0.5f;
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
        bool movePress = Input.GetKey("a") ||
                         Input.GetKey("s") || 
                         Input.GetKey("d"); 
        bool jumpPress = Input.GetKey("space");
        bool itemPress = Input.GetKey("e");

        if (itemPress)
        {
            anim.SetTrigger("Attack2");
        }
        else if (jumpPress)
        {
            anim.SetTrigger("Jump");
        }

        if (forwardPress && speed < maxSpeed)
        {
            speed += Time.deltaTime * accel;
        } 
        else if (movePress && speed < halfSpeed)
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
