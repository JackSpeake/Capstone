using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Tooltip("How close to the wall the player will automagically let go")]
    [SerializeField] private float letGoDistance = 3f;
    [Tooltip("How much the player gets sped up each frame")]
    [SerializeField] private float speedUp = 1.005f;

    private GameObject player;

    public void StartShootRoutine(GameObject playerObj)
    {
        player = playerObj;
        StartCoroutine("ShootCoroutine");
    }

    private IEnumerator ShootCoroutine()
    {

        RaycastHit hit;
        Transform objectHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {
            objectHit = hit.transform;
            Debug.Log("Cast hit: " + hit.transform.position);
            NetworkCharacterControllerPrototype playerController = player.GetComponent<NetworkCharacterControllerPrototype>();

            while ((player.transform.position - objectHit.position).sqrMagnitude > letGoDistance)
            {
                // TODO: Make player item manager know if/when the grapple starts or ends
                // this would allow the player to have an early exit.

                Vector3 dir = objectHit.position - player.transform.position;
                dir = dir.normalized;
                playerController.VelMult = speedUp;
                playerController.ShiftDirection(dir);
                yield return 0;
            }
        }

        

        yield return new WaitForEndOfFrame();

    }
}
