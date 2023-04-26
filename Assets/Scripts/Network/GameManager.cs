using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class GameManager : MonoBehaviour
{
    public static List<GameObject> players = new List<GameObject>();
    public static NetworkObject spawnBox;
    public static NetworkRunner networkRunner;

    [SerializeField] private int playersNeeded = 2;

    private bool gameInProgress = false;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count == playersNeeded && !gameInProgress)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting the game");
        if (spawnBox != null)
        {
            networkRunner.Despawn(spawnBox);
        }
        gameInProgress = true;
    }
}
