using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearCollectible : MonoBehaviour
{
    [Tooltip("Spear prefab")]
    public GameObject spearPrefab;
    [Tooltip("WaitTime")]
    public float waitBeforeRespawn = 3f;
    [Tooltip("Grapple sprite")]
    public Sprite spearSprite;
    [Tooltip("Initial velocity of the thrown spear")]
    public float initialVelocity = 5f;

    private PlayerItemManager itemManager;
    private GameObject playerWithItem;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UseSpear()
    {
        Debug.Log("Spear used");
        itemManager.useItem -= UseSpear;
        itemManager.throwItem -= ThrowSpear;
        GrappleShot();
    }

    private void ThrowSpear()
    {
        Debug.Log("Spear Thrown");
        itemManager.useItem -= UseSpear;
        itemManager.throwItem -= ThrowSpear;
        ThrowSpearFromPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();

            if (itemManager.PickUpItem(UseSpear, ThrowSpear, spearSprite))
            {
                playerWithItem = other.gameObject;

                // Make it so no one else can pick up the item, don't wont to SetActive(false) because then the script stops working
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine("CauseStartAgain");
            }
        }
    }

    private IEnumerator CauseStartAgain()
    {
        yield return new WaitForSeconds(waitBeforeRespawn);
        Respawn();
    }

    public void Respawn()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    private void GrappleShot()
    {
        Debug.Log("Grapple Shot");
        GameObject grapple = GameObject.Instantiate(spearPrefab, playerWithItem.transform.position, Quaternion.identity);
        grapple.transform.parent = playerWithItem.transform;
        grapple.GetComponent<Grapple>().StartShootRoutine(playerWithItem);
    }

    private void ThrowSpearFromPlayer()
    {
        Camera cam = playerWithItem.transform.GetComponentInChildren<Camera>();
        Vector3 initialPosition = cam.transform.position;
        Vector3 throwDirection = cam.transform.forward;
        initialPosition += throwDirection * 2;
        // float launchAngle = Mathf.Atan2(throwDirection.z, throwDirection.y);

        Vector3 absThrowDirection = new Vector3(Mathf.Abs(throwDirection.x), throwDirection.y, Mathf.Abs(throwDirection.z));
        float launchAngle;
        if (absThrowDirection.z > absThrowDirection.x)
        {
            launchAngle = Mathf.Atan2(absThrowDirection.z, absThrowDirection.y);
        } 
        else
        {
            launchAngle = Mathf.Atan2(absThrowDirection.x, absThrowDirection.y);
        }


        Debug.Log($"Spear throw direction: {throwDirection}");
        Debug.Log($"Launch Angle in degrees: {launchAngle * Mathf.Rad2Deg}");

        // GameObject spear = GameObject.Instantiate(spearPrefab, initialPosition, Quaternion.LookRotation(throwDirection));
        GameObject spear = GameObject.Instantiate(spearPrefab, initialPosition, Quaternion.LookRotation(new Vector3(throwDirection.x, 0f, throwDirection.z)));
        Spear spearScript = spear.GetComponent<Spear>();
        spearScript.Launch(initialVelocity, launchAngle);
        // spear.GetComponentInChildren<Rigidbody>().AddForce(throwDirection * initialVelocity, ForceMode.Impulse);
    }
}
