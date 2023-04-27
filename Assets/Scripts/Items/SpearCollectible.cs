using Fusion;
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
    [Tooltip("Affects the angle for the spear to be used as a ramp. The more negative the number the lower the angle becomes (flatter ramp)")]
    public float yLookDirection = -1f;

    private PlayerItemManager itemManager;
    private GameObject playerWithItem;
    public AudioClip pickup;
    public AudioClip spearToss;

    public GameObject model;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UseSpear(NetworkObject obj)
    {
        Debug.Log("Spear used");
        itemManager.useItem -= UseSpear;
        itemManager.throwItem -= ThrowSpear;
        PlaceSpearAsRamp();
    }

    private void ThrowSpear(NetworkObject obj)
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
            itemManager.spawn = false;
            itemManager.prefab = spearPrefab;

            if (itemManager.PickUpItem(UseSpear, ThrowSpear, spearSprite))
            {
                playerWithItem = other.gameObject;

                // Make it so no one else can pick up the item, don't wont to SetActive(false) because then the script stops working
                gameObject.GetComponent<BoxCollider>().enabled = false;
                model.SetActive(false);
                StartCoroutine("CauseStartAgain");
            }
            AudioSource.PlayClipAtPoint(pickup, gameObject.transform.position);
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
        model.SetActive(true);
    }

    private void PlaceSpearAsRamp()
    {
        Camera cam = playerWithItem.transform.GetComponentInChildren<Camera>();
        Vector3 initialPosition = cam.transform.position;
        Vector3 placeDirection = new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z);
        initialPosition += placeDirection * 2;
        Vector3 rotationDirection = new Vector3(placeDirection.x, yLookDirection, placeDirection.z);
        GameObject.Instantiate(spearPrefab, initialPosition, Quaternion.LookRotation(rotationDirection));
    }

    private void ThrowSpearFromPlayer()
    {
        Camera cam = playerWithItem.transform.GetComponentInChildren<Camera>();
        Vector3 initialPosition = cam.transform.position;
        Vector3 throwDirection = cam.transform.forward;
        initialPosition += throwDirection * 2;

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

        GameObject spear = GameObject.Instantiate(spearPrefab, initialPosition, Quaternion.LookRotation(new Vector3(throwDirection.x, 0f, throwDirection.z)));
        spear.GetComponent<Rigidbody>().AddForce(throwDirection * initialVelocity, ForceMode.Impulse);
        AudioSource.PlayClipAtPoint(spearToss, gameObject.transform.position);
    }
}
