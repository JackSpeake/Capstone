using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;

public class PlayerItemManager : NetworkBehaviour
{
    public delegate void OnUseItem();
    public event OnUseItem useItem;

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
                useItem?.Invoke();
            }
        }
    }
}
