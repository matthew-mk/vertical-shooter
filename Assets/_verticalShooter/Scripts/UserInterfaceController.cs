using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    public Image healthBar;

    public RectTransform pausePanel;

    //texts
    public TextMeshProUGUI levelEventMessage;

    //pause resume buttons
    public Button pauseButton;
    public Button resumeButton;

    private void Start()
    {
        healthBar.fillAmount = 1f;
        pausePanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        BusSystem.General.OnShipHit += HandleHealthChanged;
        BusSystem.General.OnGamePaused += HandleGamePaused;
        BusSystem.General.OnGameResumed += HandleGameResumed;

        //generic message call
        BusSystem.General.OnDisplayMessageGeneric += HandleDisplayMessage;

        //level events
        BusSystem.LevelEvents.OnDisplayMessageEvent += HandleShowLevelEventMessage;

        //onClick subscriptions
        pauseButton.onClick.AddListener
            (
                 HandlePauseGameClicked
            );
        //() => { HandlePauseGame(); }
        resumeButton.onClick.AddListener
            (
                () => { HandleResumeGameClicked(); }
            );
    }

    private void OnDisable()
    {
        BusSystem.General.OnShipHit -= HandleHealthChanged;
        BusSystem.General.OnGamePaused -= HandleGamePaused;
        BusSystem.General.OnGameResumed -= HandleGameResumed;

        //generic message call
        BusSystem.General.OnDisplayMessageGeneric -= HandleDisplayMessage;

        //level events
        BusSystem.LevelEvents.OnDisplayMessageEvent -= HandleShowLevelEventMessage;

        pauseButton.onClick.RemoveAllListeners();
        resumeButton.onClick.RemoveAllListeners();
    }

    //handlers
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        healthBar.fillAmount = currentHealth / (float)maxHealth;
    }

    private void HandleGamePaused()
    {
        pausePanel.gameObject.SetActive(true);
    }

    private void HandleGameResumed()
    {
        pausePanel.gameObject.SetActive(false);
    }

    private void HandleShowLevelEventMessage(int levelEventID, MessageEvent messageEvent)
    {
        levelEventMessage.gameObject.SetActive(true);
        levelEventMessage.text = "";
        StartCoroutine(
            DisplayTextButCooler(messageEvent.messageToSay,3f,
            ()=> 
            {
                levelEventMessage.gameObject.SetActive(false);
                BusSystem.LevelEvents.LevelEventFinished(levelEventID);
            })
            );
    }

    private void HandleDisplayMessage(string message, float time)
    {
        levelEventMessage.gameObject.SetActive(true);
        levelEventMessage.text = "";
        StartCoroutine(
            DisplayTextButCooler(message, time,
            () =>
            {
                levelEventMessage.gameObject.SetActive(false);
            })
            );
    }

    //button on click handlers
    private void HandlePauseGameClicked()
    {        
        BusSystem.UI.PauseGameRequest();
    }

    private void HandleResumeGameClicked()
    {        
        BusSystem.UI.ResumeGameRequest();
    }

    //coroutines
    private IEnumerator DisplayTextButCooler(string textToShow, float timeToDisplay, Action onDone)
    {
        string tempText= "";
        WaitForSeconds displayDelay = new WaitForSeconds(0.1f);
        for(int i = 0; i < textToShow.Length; i++)
        {
            tempText += textToShow[i];
            levelEventMessage.text = tempText;
            yield return displayDelay;
        }
        yield return new WaitForSeconds(timeToDisplay);
        onDone?.Invoke();
    }
}
