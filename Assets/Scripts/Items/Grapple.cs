using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Tooltip("How close to the wall the player will automagically let go")]
    [SerializeField] private float letGoDistance = 3f;
    [Tooltip("How much the player gets sped up each frame")]
    [SerializeField] private float speedUp = 1.005f;
    [Tooltip("How much the player gets slowed down each frame")]
    [SerializeField] private float slowDown = .995f;
    public AudioClip grappleSfx;
    [SerializeField] private float maxGrapple = 20f;

    private GameObject player;

    public void StartShootRoutine(GameObject playerObj)
    {
        //player = playerObj;
        player = NetworkPlayer.Local.gameObject;
        StartCoroutine("ShootCoroutine");
    }

    private IEnumerator ShootCoroutine()
    {

        RaycastHit hit;
        Transform objectHit;

        float sinceStart = Time.time;

        Camera cam = NetworkPlayer.Local.GetComponentInChildren<Camera>();
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrapple))
        {

            if (hit.transform.CompareTag("WALL") )
            {
                AudioSource.PlayClipAtPoint(grappleSfx, gameObject.transform.position);
                objectHit = hit.transform;
                Debug.Log("Cast hit: " + hit.transform.position);
                NetworkCharacterControllerPrototype playerController = player.GetComponent<NetworkCharacterControllerPrototype>();
                //AudioSource.PlayClipAtPoint(grappleSfx, objectHit.position);

                Debug.Log((player.transform.position - objectHit.position).sqrMagnitude);

                while ((player.transform.position - objectHit.position).sqrMagnitude > letGoDistance)
                {
                    // TODO: Make player item manager know if/when the grapple starts or ends
                    // this would allow the player to have an early exit.

                    if (Time.time - sinceStart > .5f && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)))
                        break;
                    
                    Vector3 dir = objectHit.position - player.transform.position;
                    dir = dir.normalized;
                    playerController.VelMult = speedUp;
                    playerController.ShiftDirection(dir);
                    yield return 0;
                }
            }
            else if (hit.transform.CompareTag("Player"))
            {
                AudioSource.PlayClipAtPoint(grappleSfx, gameObject.transform.position);
                objectHit = hit.transform;
                Debug.Log("Cast hit: " + hit.transform.position);
                NetworkCharacterControllerPrototype playerController = player.GetComponent<NetworkCharacterControllerPrototype>();

                //AudioSource.PlayClipAtPoint(grappleSfx, objectHit.position);

                while ((player.transform.position - objectHit.position).sqrMagnitude > letGoDistance)
                {
                    // TODO: Make player item manager know if/when the grapple starts or ends
                    // this would allow the player to have an early exit.

                    if (Time.time - sinceStart > .5f && Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
                        break;

                    Vector3 dir = objectHit.position - player.transform.position;
                    dir = dir.normalized;
                    playerController.VelMult = speedUp;
                    playerController.ShiftDirection(dir);

                    dir *= -1;
                    NetworkCharacterControllerPrototype otherPlayerController = hit.transform.gameObject.GetComponent<NetworkCharacterControllerPrototype>();
                    otherPlayerController.VelMult = slowDown;
                    otherPlayerController.ShiftDirection(dir);
                    yield return 0;
                }
            }    
        }


        Destroy(this.gameObject);
        yield return new WaitForEndOfFrame();

    }
}
