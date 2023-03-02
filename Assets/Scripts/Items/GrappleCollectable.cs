using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollectable : MonoBehaviour
{
    [Tooltip("Grapple prefab")]
    public GameObject grapplePrefab;

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

    private void HoldGrapple()
    {
        Debug.Log("Bomb used");
        itemManager.useItem -= HoldGrapple;
        GrappleShot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();
            itemManager.useItem += HoldGrapple;
            playerWithItem = other.gameObject;

            // Make it so no one else can pick up the item, don't wont to SetActive(false) because then the script stops working
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void GrappleShot()
    {
        Debug.Log("Grapple Shot");
        GameObject grapple = GameObject.Instantiate(grapplePrefab, playerWithItem.transform.position, Quaternion.identity);
        grapple.transform.parent = playerWithItem.transform;
        grapple.GetComponent<Grapple>().StartShootRoutine(playerWithItem);


        // after the item has been used, get rid of the collectible
        gameObject.SetActive(false);
    }
}
