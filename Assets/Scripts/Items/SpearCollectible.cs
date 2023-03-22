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
    [Tooltip("Force to throw spear")]
    public float throwForce = 5f;

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
        Vector3 initialPosition = Camera.main.transform.position;
        Vector3 throwDirection = Camera.main.transform.forward;
        initialPosition += throwDirection;
        Debug.Log(throwDirection);
        GameObject spear = GameObject.Instantiate(spearPrefab, initialPosition, Quaternion.identity);
        spear.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);
    }
}
