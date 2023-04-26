using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class GameManager : NetworkBehaviour
{
    public static List<GameObject> players = new List<GameObject>();

    public static float raceTime = 0f;
    public static bool gameInProgress = false;
    public static bool countdownStarted = false;

    public delegate void OnCountdownStarted();
    public delegate void OnCountdownTimeChanged(float time);
    public delegate void OnCountdownEnded();
    public delegate void OnRaceTimerChanged(float time);
    public delegate void OnRaceStarted();
    public static event OnCountdownStarted onCountdownStarted;
    public static event OnCountdownTimeChanged onCountdownTimeChanged;
    public static event OnCountdownEnded onCountdownEnded;
    public static event OnRaceTimerChanged onRaceTimerChanged;
    public static event OnRaceStarted onRaceStarted;

    [SerializeField] private GameObject spawnBoxPrefab;
    [SerializeField] private Vector3 spawnBoxPosition;

    [SerializeField] private int playersNeeded = 2;
    [SerializeField] private int countdownSeconds = 10;
    private Tick initialTick;
    private static TickTimer countdownTimer;
    private static NetworkObject spawnBox;

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
        }
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
