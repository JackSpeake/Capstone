using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    NetworkRunner networkRunner;
    public List<SessionInfo> sessions;

    // Start is called before the first frame update
    void Start()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();

        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network Runner";

            if (SceneManager.GetActiveScene().name != "MainMenu")
            { 
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            }

            Debug.Log($"Server NetworkRunner Started.");
        }
    }


    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // Handle Networked Objects that Already Exist
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            Initialized = initialized,
            //CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager
            
        });
    }

    public void OnJoinLobby()
    {
        //var clientTask = JoinLobby();
        bool found = false;

        if (sessions != null)
        {
            foreach (SessionInfo s in sessions)
            {
                if (!found && s.PlayerCount < 2)
                {
                    JoinGame(s);
                    found = true;
                }
            }
        }
        else
        {
            found = true;
            CreateGame("Room0", "SampleScene");
        }

        if (!found)
        {
            CreateGame("Room" + sessions.Count().ToString(), "SampleScene");
        }

    }

    private async Task JoinLobby()
    {
        string lobbyID = "OurLobbyID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.LogError("Unable to join lobby");
        }
        else
        {
            Debug.Log("Lobby Joined");
        }
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath("Scenes/SampleScene"), null);
    }

    public void JoinGame(SessionInfo info)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, info.Name, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }
}
