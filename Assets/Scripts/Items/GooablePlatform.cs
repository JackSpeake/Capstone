using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GooablePlatform : MonoBehaviour
{
    public bool gooed = false;
    public float speedChange = 1f;
    private float cooldown = 3f;
    private Color oldColor;

    bool stayGooed = false;
    
    public void HitWithGoo(float speedChange, float cooldown, Color newColor)
    {
        this.gooed = true;
        this.speedChange = speedChange;
        this.cooldown = cooldown;
        this.oldColor = this.GetComponent<MeshRenderer>().material.color;
        this.GetComponent<MeshRenderer>().material.color = newColor;

        StartCoroutine("resetPlatform");
    }

    private IEnumerator resetPlatform()
    {
        yield return new WaitForSeconds(cooldown);
        

        while (stayGooed)
        {
            yield return new WaitForFixedUpdate();
        }

        gooed = false;
        speedChange = 1f;
        this.GetComponent<MeshRenderer>().material.color = oldColor;
    }

    public void LandedPlatform()
    {
        stayGooed = true;
    }

    public void LeftPlatform()
    {
        stayGooed = false;
    }

}
