using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool paused = false;
    [SerializeField] private GameObject pauseMenu;
    public Text title;
    private float time;

    public void Update()
    {
        if (Time.time - time > 1)
        {
            time = Time.time;
            if (title.text == "Paused_")
                title.text = "Paused";
            else
                title.text = "Paused_";
        }
    }

    public void QuitToMenu()
    {
        NetworkPlayer.Local.Disconnect();   
    }

    public void TogglePause()
    {
        paused = !paused;
        pauseMenu.SetActive(paused);

        if (paused)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
