using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvasTimersManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI raceTimerText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI placementText;
    private Placement finishPlacement;

    private void Awake()
    {
        GameManager.onCountdownStarted += StartCountdownTimer;
        GameManager.onCountdownEnded += StopCountdownTimer;
        GameManager.onCountdownTimeChanged += UpdateCountdownTimer;
        GameManager.onRaceTimerChanged += UpdateTimer;
        GameManager.onRaceStarted += StartTimer;
        GameManager.onRaceEnded += DisplayFinishTime;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCountdownTimer(float time)
    {
        timeText.SetText($"{Mathf.FloorToInt(time)}");
    }

    public void StartCountdownTimer()
    {
        timeText.gameObject.SetActive(true);
    }

    public void StopCountdownTimer()
    {
        timeText.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        raceTimerText.transform.parent.gameObject.SetActive(true);
    }

    public void PlayerFinished(Placement finishPlacement)
    {
        this.finishPlacement = finishPlacement;
        placementText.SetText("Finished!");
        placementText.gameObject.SetActive(true);
    }

    public void DisplayFinishTime()
    {
        int minutes = Mathf.FloorToInt(finishPlacement.finishTime / 60);
        int seconds = Mathf.FloorToInt(finishPlacement.finishTime % 60);
        if (finishPlacement.place == 1)
        {
            Debug.Log($"{transform.name} has won!");
            placementText.SetText($"You Won!");

        }
        else if (finishPlacement.place == 2)
        {
            Debug.Log($"{transform.name} has lost :(");
            placementText.SetText("You Lost");
        }
        timeText.SetText($"Your time: {minutes}:{seconds:D2}");
        timeText.gameObject.SetActive(true);
    }

    private void UpdateTimer(float raceTime)
    {
        int minutes = Mathf.FloorToInt(raceTime / 60);
        int seconds = Mathf.FloorToInt(raceTime % 60);
        raceTimerText.SetText($"{minutes}:{seconds:D2}");
    }
}
