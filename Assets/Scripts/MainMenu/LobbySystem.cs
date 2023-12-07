using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySystem : MonoBehaviour
{
    [SerializeField] private GameObject startPanel, mainMenuPanel, newGamePanel;
    public TMPro.TMP_InputField sessionNameInputField, maxPlayersInputField;
    public TMPro.TextMeshProUGUI statusText;
    public TMPro.TMP_Dropdown mapDropdown;

    public void startClick()
    {
        startPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobbySpecific();
        FindObjectOfType<SessionListUIHandler>(true).OnLookingForGameSessions();
    }

    public void returnClick()
    {
        startPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        statusText.transform.gameObject.SetActive(false);
    }

    public void createClick()
    {
        startPanel.SetActive(false);
        newGamePanel.SetActive(true);
        statusText.transform.gameObject.SetActive(false);
    }

    public void returnCreateGameClick()
    {
        startPanel.SetActive(true);
        newGamePanel.SetActive(false);
        statusText.transform.gameObject.SetActive(true);
    }

    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        int number;

        bool success = int.TryParse(maxPlayersInputField.text, out number);

        string chosenMap = "Scenes/SampleScene";

        Debug.Log(mapDropdown.value);

        switch(mapDropdown.value)
        {
            case 0:
                chosenMap = "Scenes/SampleScene";
                break;
            case 1:
                chosenMap = "Scenes/LevelTwo";
                break;
            case 2:
                chosenMap = "Scenes/LevelThree";
                break;
        }

        if (success && number < 9 && number > 0)
            networkRunnerHandler.CreateGame(sessionNameInputField.text, chosenMap, number);
        else
            networkRunnerHandler.CreateGame(sessionNameInputField.text, chosenMap);
    }

    public void OnTutorialClick()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.CreateGame(sessionNameInputField.text, "Scenes/Tutorial", 1);
    }
}
