using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using UnityEngine.UI;

public class PlayerItemManager : NetworkBehaviour
{
    public delegate void OnSelfUseItem();
    public delegate void OnThrowItem();
    public event OnSelfUseItem useItem;
    public event OnThrowItem throwItem;
    public Image itemImage;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.selfItemPressed)
            {
                Debug.Log("Using item");
                itemImage.sprite = null;
                useItem?.Invoke();
            }

            if (networkInputData.throwItemPressed)
            {
                Debug.Log("Throwing Item");
                itemImage.sprite = null;
                throwItem?.Invoke();
            }
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
