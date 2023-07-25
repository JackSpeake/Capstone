using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector3 aimForwardVector; 
    public bool jumpPressed;
    public bool jumpHeld;
    public bool dashPressed;
    public bool selfItemPressed;
    public bool throwItemPressed;
}
