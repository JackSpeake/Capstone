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
    [Tooltip("Bomb item sprite")]
    public Sprite bombSprite;

    private PlayerItemManager itemManager;
    private GameObject playerWithItem;
    private Bomb createdBombScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlaceBomb(NetworkObject obj)
    {
        Debug.Log("Bomb used");
        itemManager.spawn = false;
        itemManager.useItem -= PlaceBomb;
        itemManager.throwItem -= ThrowBomb;
        PlacedBombExplosion(obj);
    }

    private void ThrowBomb(NetworkObject obj)
    {
        Debug.Log("Bomb thrown");
        itemManager.spawn = false;
        itemManager.throwItem -= ThrowBomb;
        itemManager.useItem -= PlaceBomb;
        //StartCoroutine(ThrownBombExplosion());
        ThrownBombExplosion(obj);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            itemManager = other.gameObject.GetComponent<PlayerItemManager>();
            itemManager.prefab = bombPrefab;
            itemManager.spawn = true;

            //itemManager.useItem += PlaceBomb;
            //itemManager.throwItem += ThrowBomb;
            if (itemManager.PickUpItem(PlaceBomb, ThrowBomb, bombSprite))
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


    private void PlacedBombExplosion(NetworkObject obj)
    {
        Debug.Log("Bomb Placed");
        createdBombScript = obj.GetComponent<Bomb>();
        createdBombScript.SetExplosionRadius(placedExposionRadius);
        createdBombScript.CreateExplosionRadiusIndicator();
        itemManager.useItem += ExplodeCreatedBomb;
        itemManager.throwItem += ExplodeCreatedBomb;
        itemManager.spawn = false;

        // after the item has been used, get rid of the collectible
        //gameObject.SetActive(false);
    }

    private void ThrownBombExplosion(NetworkObject obj)
    {
        Vector3 throwDirection = NetworkPlayer.Local.transform.forward;
        Debug.Log("Bomb Thrown");
        createdBombScript = obj.GetComponent<Bomb>();
        obj.GetComponent<Rigidbody>().AddForce(throwDirection * throwForce, ForceMode.Impulse);
        createdBombScript.SetExplosionRadius(thrownExplosionRadius);
        createdBombScript.CreateExplosionRadiusIndicator();
        itemManager.spawn = false;

        itemManager.useItem += ExplodeCreatedBomb;
        itemManager.throwItem += ExplodeCreatedBomb;
    }

    private void ExplodeCreatedBomb(NetworkObject obj)
    {
        createdBombScript.ExplodeBomb();
        itemManager.useItem -= ExplodeCreatedBomb;
        itemManager.throwItem -= ExplodeCreatedBomb;
    }
}
