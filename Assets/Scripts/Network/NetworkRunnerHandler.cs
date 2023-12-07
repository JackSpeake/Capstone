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

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, hostMigrationToken);

        Debug.Log("Host Migration Started");
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            //SceneManager = sceneManager
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = HostMigrationResume,
            //ConnectionToken = GameManager.instance.GetConnectionToken
        });
    }

    void HostMigrationResume(NetworkRunner runner)
    {
        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterControllerPrototype>(out var characterController))
            {
                // do later fix later :3
            }
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

        FindObjectOfType<Spawner>().MaxPlayers = 2;

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            PlayerCount = 2,
            Initialized = initialized,
            CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager
            
        });
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized, int maxPlayers)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // Handle Networked Objects that Already Exist
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;
        FindObjectOfType<Spawner>().MaxPlayers = maxPlayers;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            PlayerCount = maxPlayers,
            SessionName = sessionName,
            Initialized = initialized,
            CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager

        });
    }

    public IEnumerator OnJoinLobby()
    {
        bool found = false;
        int count = 0;
        while (sessions == null && count < 5)
        {
            yield return new WaitForSeconds(1f);
            count++;
        }

        if (sessions != null)
        {
            Debug.Log("TEST");
            foreach (SessionInfo s in sessions)
            {
                if (!found && s.PlayerCount < 2)
                {
                    JoinGame(s);
                    Debug.Log("FUDCKSJDAKJDSA");
                    found = true;
                }
            }
        }
        else
        {
            Debug.Log("DEAD");
            found = true;
            CreateGame("Room0", "SampleScene");
        }

        if (!found)
        {
            CreateGame("Room" + sessions.Count().ToString(), "SampleScene");
        }

    }

    public void OnJoinLobbySpecific()
    {
        var clientTask = JoinLobbySpecific();
    }


    public async void StartGame()
    {
        await JoinLobby();
    }

    public async Task JoinLobby()
    {
        string lobbyID = "OurLobbyID";

        //var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);
        //if (!result.Ok)
        //{
        //    Debug.LogError("Unable to join lobby");
        //}
        //else
        //{
         //   Debug.Log("Lobby Joined");
            StartCoroutine("OnJoinLobby");
        //}
    }

    public async Task JoinLobbySpecific()
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
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath(sceneName), null);
    }

    public void CreateGame(string sessionName, string sceneName, int maxPlayers)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath(sceneName), null, maxPlayers);
    }

    public void JoinGame(SessionInfo info)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, info.Name, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }

    IEnumerator CleanUpHostMigrationCO()
    {
        yield return new WaitForSeconds(5.0f);

        //FindObjectOfType<Spawner>().OnHostMigrationCleanUp();
    }
}
