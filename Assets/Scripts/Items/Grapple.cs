using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{

    public void StartShootRoutine()
    {
        StartCoroutine("ShootCoroutine");
    }

    private IEnumerator ShootCoroutine()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Debug.Log("Cast hit: " + hit.transform.position);

            // Do something with the object that was hit by the raycast.
        }

        yield return new WaitForEndOfFrame();

    }
}
