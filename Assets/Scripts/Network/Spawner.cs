using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Sockets;
using Fusion;
using System;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;
    public GameObject gameManager;
    CharacterInputHandler characterInputHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined we are server, spawning player.");
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
        }
        else
        {
            Debug.Log("Method Call: OnPlayerJoined");
        }

        GameObject[] objects = GameObject.FindGameObjectsWithTag("COLLECTABLE");
        foreach (GameObject o in objects)
        {
            if (o.GetComponent<BombCollectible>() != null)
            {
                o.GetComponent<BombCollectible>().Respawn();
            }
            else if (o.GetComponent<GrappleCollectable>() != null)
            {
                o.GetComponent<GrappleCollectable>().Respawn();
            }
            else if (o.GetComponent<SpearCollectible>() != null)
            {
                o.GetComponent<SpearCollectible>().Respawn();
            }
        }
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Method Call: OnPlayerLeft");
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        //Debug.Log("Method Call: OnInput");
        if (characterInputHandler == null && NetworkPlayer.Local != null)
        {
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }

        if (characterInputHandler != null)
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }

    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("Method Call: OnInputMissing");
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Method Call: OnShutdown");
    }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Method Call: OnConnectedToServer");
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Method Call: OnDisconnectedFromServer");
        SceneManager.LoadScene(0);
        
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Method Call: OnConnectRequest");
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Method Call: OnConnectFailed");
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("Method Call: OnUserSimulationMessage");
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        FindObjectOfType<NetworkRunnerHandler>().sessions = sessionList;
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Method Call: OnCustomAuthenticationResponse");
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Method Call: OnHostMigration");
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("Method Call: OnReliableDataReceived");
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Method Call: OnSceneLoadDone");
        NetworkObject spawnedGameManager = runner.Spawn(gameManager);
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Method Call: OnSceneLoadStart");
    }
}
