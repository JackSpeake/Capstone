using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombCollectible : MonoBehaviour
{
    [Tooltip("The number of seconds before the bomb will explode after being placed (self use)")]
    public float placedSecondsBeforeExplosion = 1f;
    [Tooltip("The number of seconds before the bomb will explode after being thrown (disrupt enemy use)")]
    public float thrownSecondsBeforeExplosion = 5f;
    [Tooltip("The radius of the explosion when the bomb is placed (self use)")]
    public float placedExposionRadius = 5f;
    [Tooltip("The radius of the explosion when the bomb is thrown (disrupt enemy use)")]
    public float thrownExplosionRadius = 15f;
    [Tooltip("Force to throw the bomb")]
    public float throwForce = 5f;
    [Tooltip("Bomb prefab")]
    public GameObject bombPrefab;
    [Tooltip("WaitTime")]
    public float waitBeforeRespawn = 3f;

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

    private void PlaceBomb()
    {
        Debug.Log("Bomb used");
        itemManager.useItem -= PlaceBomb;
        StartCoroutine(PlacedBombExplosion());
    }

    private void ThrowBomb()
    {
        Debug.Log("Bomb thrown");
        itemManager.throwItem -= ThrowBomb;
        StartCoroutine(ThrownBombExplosion());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();
            itemManager.useItem += PlaceBomb;
            itemManager.throwItem += ThrowBomb;
            playerWithItem = other.gameObject;

            // Make it so no one else can pick up the item, don't wont to SetActive(false) because then the script stops working
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine("CauseStartAgain");
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


    private IEnumerator PlacedBombExplosion()
    {
        Debug.Log("Bomb Placed");
        Vector3 initialPosition = new Vector3(playerWithItem.transform.position.x, 0f, playerWithItem.transform.position.z);
        GameObject bomb = GameObject.Instantiate(bombPrefab, initialPosition, Quaternion.identity);
        yield return new WaitForSeconds(placedSecondsBeforeExplosion);
        bomb.GetComponent<Bomb>().ExplodeBomb(placedExposionRadius);
        // after the item has been used, get rid of the collectible
        gameObject.SetActive(false);
    }

    private IEnumerator ThrownBombExplosion()
    {
        Debug.Log("Bomb Thrown");
        Vector3 initialPosition = Camera.main.transform.position;
        Vector3 throwDirection = Camera.main.transform.forward;
        Debug.Log(throwDirection);
        GameObject bomb = GameObject.Instantiate(bombPrefab, initialPosition, Quaternion.identity);
        bomb.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);
        yield return new WaitForSeconds(thrownSecondsBeforeExplosion);
        bomb.GetComponent<Bomb>().ExplodeBomb(placedExposionRadius);
        // after the item has been used, get rid of the collectible
        gameObject.SetActive(false);
    }
}
