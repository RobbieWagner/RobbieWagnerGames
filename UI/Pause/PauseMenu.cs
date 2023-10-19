using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : Menu
{

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] List<string> actionMapsToDisable;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas controls;

    [HideInInspector] public bool canPause;
    [HideInInspector] public bool paused;

    public static PauseMenu Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 

        canPause = true;
        paused = false;
    } 

    protected override void OnEnable()
    {
            base.OnEnable();

            paused = true;
            Time.timeScale = 0;

            foreach(string actionMapName in actionMapsToDisable)
            {
                if(!string.IsNullOrEmpty(actionMapName))
                {
                    playerInput.actions.FindActionMap(actionMapName).Disable();
                } 
            }

            resumeButton.onClick.AddListener(ResumeGame);
            settingsButton.onClick.AddListener(OpenSettings);
            controlsButton.onClick.AddListener(OpenControls);
            saveButton.onClick.AddListener(SaveGame);
            quitButton.onClick.AddListener(QuitToMainMenu);

            thisCanvas.enabled = true;

            if(PlayerMovement.Instance != null) 
            {
                PlayerMovement.Instance.canMove = false;
                PlayerMovement.Instance.StopPlayer();
            }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if(!paused) 
        {
            Time.timeScale = 1;
            foreach(string actionMapName in actionMapsToDisable)
            {
                if(!string.IsNullOrEmpty(actionMapName))
                {
                    playerInput.actions.FindActionMap(actionMapName).Enable();
                } 
            }

            if(PlayerMovement.Instance != null) 
            {
                PlayerMovement.Instance.canMove = true;
                PlayerMovement.Instance.StopPlayer();
            }
        }

        resumeButton.onClick.RemoveListener(ResumeGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        controlsButton.onClick.RemoveListener(OpenControls);
        saveButton.onClick.RemoveListener(SaveGame);
        quitButton.onClick.RemoveListener(QuitToMainMenu);

        thisCanvas.enabled = false;
    }

    public void ResumeGame()
    {
        paused = false;
        this.enabled = false;
    }

    private void OpenSettings()
    {
        StartCoroutine(SwapCanvases(thisCanvas, settings));
    }

    private void OpenControls()
    {
        StartCoroutine(SwapCanvases(thisCanvas, controls));
    }

    private void SaveGame()
    {
        StartCoroutine(GameManager.Instance.SaveGame());

        ToggleButtonInteractibility(false);
        GameManager.Instance.OnSaveGame += OnSaveButtonComplete;
    }

    private void OnSaveButtonComplete()
    {
        ToggleButtonInteractibility(true);
        GameManager.Instance.OnSaveGame -= OnSaveButtonComplete;
    }

    private void QuitToMainMenu()
    {
        ToggleButtonInteractibility(false);

        StartCoroutine(QuitToMainMenuCo());
    }

    protected override void ToggleButtonInteractibility(bool toggleOn)
    {
        base.ToggleButtonInteractibility(toggleOn);

        resumeButton.interactable = toggleOn;
        settingsButton.interactable = toggleOn;
        controlsButton.interactable = toggleOn;
        saveButton.interactable = toggleOn;
        quitButton.interactable = toggleOn;
    }

    private IEnumerator QuitToMainMenuCo()
    {
        yield return new WaitForSecondsRealtime(.1f);
        Time.timeScale = 1;
        GameManager.Instance.UpdateGameState(GameState.None);
        SceneManager.LoadScene("MainMenu");

        StopCoroutine(QuitToMainMenuCo());
    }

    protected override IEnumerator SwapCanvases(Canvas active, Canvas next)
    {
        yield return StartCoroutine(base.SwapCanvases(active, next));

        StopCoroutine(SwapCanvases(active, next));
    }
}
