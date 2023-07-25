using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCanvasTimersManager : MonoBehaviour
{
    public delegate void OnPlayAgainPressed();
    public static event OnPlayAgainPressed onPlayAgainPressed;

    [SerializeField] private TextMeshProUGUI raceTimerText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI placementText;
    [SerializeField] private Button playAgainButtton;
    public AudioClip win;
    public AudioClip lose;
    private Placement finishPlacement;
    private bool waitingForOtherPlayer = false;

    private void Awake()
    {
        GameManager.onCountdownStarted += StartCountdownTimer;
        GameManager.onCountdownEnded += StopCountdownTimer;
        GameManager.onCountdownTimeChanged += UpdateCountdownTimer;
        GameManager.onRaceTimerChanged += UpdateTimer;
        GameManager.onRaceStarted += StartTimer;
        GameManager.onRaceEnded += DisplayFinishTime;
        GameManager.onRaceReset += ResetCanvas;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAgainPressed()
    {
        timeText.gameObject.SetActive(false);
        playAgainButtton.gameObject.SetActive(false);
        waitingForOtherPlayer = true;
        StartCoroutine(WaitingForOtherPlayerAnimation()); 
        Cursor.lockState = CursorLockMode.None;
        onPlayAgainPressed.Invoke();
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

    public void ResetCanvas()
    {
        raceTimerText.transform.parent.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        placementText.gameObject.SetActive(false);
        playAgainButtton.gameObject.SetActive(false);
        finishPlacement.finishTime = 0f;
        finishPlacement.place = 0;
        waitingForOtherPlayer = false;
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
            AudioSource.PlayClipAtPoint(win, gameObject.transform.position);
       }
        else if (finishPlacement.place == 2)
        {
            Debug.Log($"{transform.name} has lost :(");
            placementText.SetText("You Lost");
            AudioSource.PlayClipAtPoint(lose, gameObject.transform.position);
        }
        timeText.SetText($"Your time: {minutes}:{seconds:D2}");
        timeText.gameObject.SetActive(true);
        playAgainButtton.gameObject.SetActive(true);
    }

    private void UpdateTimer(float raceTime)
    {
        int minutes = Mathf.FloorToInt(raceTime / 60);
        int seconds = Mathf.FloorToInt(raceTime % 60);
        raceTimerText.SetText($"{minutes}:{seconds:D2}");
    }

    private IEnumerator WaitingForOtherPlayerAnimation()
    {
        int dots = 1;
        while (waitingForOtherPlayer)
        {
            if (dots == 1)
            {
                placementText.SetText("Waiting for other player.");
            }
            else if (dots == 2)
            {
                placementText.SetText("Waiting for other player..");
            }
            else
            {
                placementText.SetText("Waiting for other player...");
            }
            dots = (dots + 1) % 3;
            yield return new WaitForSeconds(1f);
        }
    }
}
