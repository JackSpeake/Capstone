using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollectable : MonoBehaviour
{
    [Tooltip("Grapple prefab")]
    public GameObject grapplePrefab;
    [Tooltip("WaitTime")]
    public float waitBeforeRespawn = 3f;
    [Tooltip("Grapple sprite")]
    public Sprite grappleSprite;
    public AudioClip pickup;
    public AudioClip grappleSound;

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

    private void HoldGrapple(NetworkObject obj)
    {
        Debug.Log("Grapple used");
        itemManager.useItem -= HoldGrapple;
        GrappleShot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();
            itemManager.spawn = false;

            if (itemManager.PickUpItem(HoldGrapple, null, grappleSprite))
            {
                playerWithItem = other.gameObject;

                // Make it so no one else can pick up the item, don't wont to SetActive(false) because then the script stops working
                gameObject.GetComponent<BoxCollider>().enabled = false;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
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
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    private void GrappleShot()
    {
        Debug.Log("Grapple Shot");
        GameObject grapple = GameObject.Instantiate(grapplePrefab, playerWithItem.transform.position, Quaternion.identity);
        grapple.transform.parent = playerWithItem.transform;
        grapple.GetComponent<Grapple>().StartShootRoutine(playerWithItem);
        AudioSource.PlayClipAtPoint(pickup, GameObject.FindGameObjectWithTag("MainCamera").transform.position);


        // after the item has been used, get rid of the collectible
        // gameObject.SetActive(false);
    }
}
