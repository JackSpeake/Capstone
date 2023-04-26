using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public struct Placement
{
    public float finishTime { get; set; }
    public int place { get; set; }

    public Placement(float finishTime, int place)
    {
        this.finishTime = finishTime;
        this.place = place;
    }
}

public class GameManager : NetworkBehaviour
{
    public static bool gameInProgress = false;
    public static bool countdownStarted = false;

    public delegate void OnCountdownStarted();
    public delegate void OnCountdownTimeChanged(float time);
    public delegate void OnCountdownEnded();
    public delegate void OnRaceTimerChanged(float time);
    public delegate void OnRaceStarted();
    public delegate void OnRaceEnded();
    public static event OnCountdownStarted onCountdownStarted;
    public static event OnCountdownTimeChanged onCountdownTimeChanged;
    public static event OnCountdownEnded onCountdownEnded;
    public static event OnRaceTimerChanged onRaceTimerChanged;
    public static event OnRaceStarted onRaceStarted;
    public static event OnRaceEnded onRaceEnded;

    [SerializeField] private GameObject spawnBoxPrefab;
    [SerializeField] private Vector3 spawnBoxPosition;

    [SerializeField] private int playersNeeded = 2;
    [SerializeField] private int countdownSeconds = 10;
    private Tick initialTick;
    private static TickTimer countdownTimer;
    private static NetworkObject spawnBox;
    private static List<GameObject> players = new List<GameObject>();
    private int numPlayersFinished = 0;
    [Networked] public bool gameFinished { get; set; }

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

    }

    public override void Spawned()
    {
        NetworkObject spawnedBox = Runner.Spawn(spawnBoxPrefab, spawnBoxPosition, Quaternion.identity);
        if (spawnedBox != null)
        {
            spawnBox = spawnedBox;
        }
        PlayerFinishManager.onPlayerFinished += GetFinishPlace;
    }

    public override void FixedUpdateNetwork()
    {
        if (players.Count == playersNeeded && !gameInProgress && !countdownStarted)
        {
            StartCoroutine(StartRaceCountdown());
        }

        if (gameInProgress)
        {
            Tick elapsedTicks = Runner.Simulation.Tick - initialTick;
            onRaceTimerChanged.Invoke(elapsedTicks / 60f);

            //Debug.Log(numPlayersFinished);

            if (numPlayersFinished == playersNeeded || gameFinished)
            {
                gameFinished = true;
                Debug.Log("The race is over");
                gameInProgress = false;
                onRaceEnded.Invoke();
            }
        }
    }

    public static void AddPlayer(GameObject player)
    {
        players.Add(player);
    }

    public void StartGame()
    {
        if (spawnBox != null)
        {
            Runner.Despawn(spawnBox);
        }
        onRaceStarted.Invoke();
        gameInProgress = true;
        initialTick = Runner.Simulation.Tick;
    }

    public Placement GetFinishPlace()
    {
        numPlayersFinished++;
        Tick elapsedTicks = Runner.Simulation.Tick - initialTick;
        return new Placement(elapsedTicks / 60f, numPlayersFinished);
    }

    private IEnumerator StartRaceCountdown()
    {
        countdownTimer = TickTimer.CreateFromSeconds(Runner, countdownSeconds);
        countdownStarted = true;
        onCountdownStarted.Invoke();
        for (int i = countdownSeconds; i > 0; i--)
        {
            onCountdownTimeChanged.Invoke(countdownTimer.RemainingTime(Runner).Value);
            yield return new WaitForSeconds(1f);
        }
        onCountdownEnded.Invoke();
        StartGame();
    }
}
