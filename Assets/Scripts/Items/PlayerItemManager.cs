using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using UnityEngine.UI;

public class PlayerItemManager : NetworkBehaviour
{
    public delegate void OnSelfUseItem(NetworkObject obj);
    public delegate void OnThrowItem(NetworkObject obj);
    public event OnSelfUseItem useItem;
    public event OnThrowItem throwItem;
    public GameObject prefab;
    public Image itemImage;
    public bool spawn = true;
    public Sprite noItemImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData) && NetworkPlayer.Local.Runner.IsServer && prefab)
        {
            NetworkObject obj = null;
            if (networkInputData.selfItemPressed)
            {
                Debug.Log("Using item");
                itemImage.sprite = noItemImage;
                if (spawn)
                    obj = NetworkPlayer.Local.Runner.Spawn(prefab, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
                useItem?.Invoke(obj);
            }

            if (networkInputData.throwItemPressed)
            {
                if (spawn)
                {
                    Vector3 initialPosition = transform.position;
                    obj = NetworkPlayer.Local.Runner.Spawn(prefab, initialPosition, Quaternion.identity);
                    Debug.Log("Throwing Item");
                }
                
                itemImage.sprite = noItemImage;
                throwItem?.Invoke(obj);
            }
        }
        else if (prefab && prefab.GetComponent<Grapple>())
        {
            NetworkObject obj = null;
            useItem?.Invoke(obj);
        }
    }

    public bool PickUpItem(OnSelfUseItem selfUseFunc, OnThrowItem throwItemFunc, Sprite image)
    {
        // Don't pick up the item if the player already has one
        if (useItem != null && useItem.GetInvocationList().Length > 0)
        {
            return false;
        }

        useItem += selfUseFunc;
        throwItem += throwItemFunc;
        itemImage.sprite = image;
        Debug.Log("Picked something up");
        return true;
    }
}
