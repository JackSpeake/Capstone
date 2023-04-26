using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvasTimersManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI countdownText;
    private bool showCountdown;

    private void Awake()
    {
        GameManager.onCountdownStarted += StartCountdownTimer;
        GameManager.onCountdownEnded += StopCountdownTimer;
        GameManager.onCountdownTimeChanged += UpdateCountdownTimer;
        GameManager.onRaceTimerChanged += UpdateTimer;
        GameManager.onRaceStarted += StartTimer;
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
        countdownText.SetText(string.Format("{0}", Mathf.FloorToInt(time)));
    }

    public void StartCountdownTimer()
    {
        countdownText.gameObject.SetActive(true);
    }

    public void StopCountdownTimer()
    {
        countdownText.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        timerText.transform.parent.gameObject.SetActive(true);
    }

    private void UpdateTimer(float raceTime)
    {
        int minutes = Mathf.FloorToInt(raceTime / 60);
        int seconds = Mathf.FloorToInt(raceTime % 60);
        timerText.SetText(string.Format("{0}:{1:D2}", minutes, seconds));
    }
}
