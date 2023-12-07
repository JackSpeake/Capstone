using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        statusText.transform.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListUIItem addedSessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();

        addedSessionInfoListUIItem.SetInformation(sessionInfo);

        addedSessionInfoListUIItem.OnJoinSession += addedSessionInfoListUIItem_OnJoinSession;
    }

    private void addedSessionInfoListUIItem_OnJoinSession(SessionInfo obj)
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.JoinGame(obj);
    }

    public void OnNoSessionFound()
    {
        ClearList();
        statusText.text = "No Game Sessions Found";
        statusText.transform.gameObject.SetActive(true);
    }

    public void OnLookingForGameSessions()
    {
        ClearList();
        statusText.text = "Looking for Game Sessions";
        statusText.transform.gameObject.SetActive(true);
    }
}
