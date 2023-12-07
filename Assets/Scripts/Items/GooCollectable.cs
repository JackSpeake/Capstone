using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooCollectable : MonoBehaviour
{
    [SerializeField] public float throwForce = 5f;
    [SerializeField] public float waitBeforeRespawn = 3f;
    [SerializeField] public Sprite gooSprite;
    [SerializeField] public AudioClip pickup;
    [SerializeField] public GameObject gooPrefab;

    private PlayerItemManager itemManager;
    private GameObject playerWithItem;
    private Goo createdGooScript;

    [SerializeField] public GameObject model;

    private void ThrowGooFast(NetworkObject obj)
    {
        Debug.Log("Fast Goo Thrown");
        itemManager.spawn = false;
        itemManager.useItem -= ThrowGooFast;
        itemManager.throwItem -= ThrowGooSlow;
        createdGooScript = obj.GetComponent<Goo>();
        Camera cam = playerWithItem.transform.GetComponentInChildren<Camera>();
        Vector3 throwFull = cam.transform.forward * throwForce;
        createdGooScript.Setup(true, throwFull);
    }

    private void ThrowGooSlow(NetworkObject obj)
    {
        Debug.Log("Slow Goo Thrown");
        itemManager.spawn = false;
        itemManager.throwItem -= ThrowGooSlow;
        itemManager.useItem -= ThrowGooFast;
        createdGooScript = obj.GetComponent<Goo>();
        Camera cam = playerWithItem.transform.GetComponentInChildren<Camera>();
        Vector3 throwFull = cam.transform.forward * throwForce;
        createdGooScript.Setup(false, throwFull);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();
            itemManager.prefab = gooPrefab;
            itemManager.spawn = true;

            //itemManager.useItem += PlaceBomb;
            //itemManager.throwItem += ThrowBomb;
            if (itemManager.PickUpItem(ThrowGooFast, ThrowGooSlow, gooSprite))
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
}
