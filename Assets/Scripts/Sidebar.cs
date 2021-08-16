using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sidebar : MonoBehaviour
{

    public static Sidebar instance;
    public GameObject dialogueSidebarContainer;

    public List<Sprite> sprites;

    public float actionCooldown = 3;
    private float currentCooldown = 0;

    public enum Menu { Main, Action, Grenade, Confirm };

    public GameObject pauseMenu;
    public List<Button> allButtons;
    public List<Button> actionConfirmButtons;
    public List<Button> moveConfirmButtons;
    public List<Button> zoomButtons;
    public Button actionButton;
    public Image actionIcon;

    public List<Image> moveIcons;
    public Image pauseDisplay;
    private MenuNavigator menuNavigator;

    public List<int> cameraZooms;
    private int currentZoom = 0;

    private static bool canAttack = true;
    private static bool menuPaused = false;
    private static bool actionPaused = false;
    private static bool running = false;

    private Zoomer zoomer;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ResetStatics();

        menuNavigator = GetComponent<MenuNavigator>();
        zoomer = Camera.main.GetComponent<Zoomer>();
        currentZoom = 0;
        if(zoomer)
            ZoomOut();
        RefreshRunning();
    }

    // Update is called once per frame
    void Update()
    {
        currentCooldown -= Time.deltaTime;
        actionButton.interactable = canAttack = currentCooldown <= 0;
        actionIcon.fillAmount = 1.0f - Mathf.Max(currentCooldown / actionCooldown, 0);
    }

    private void ResetStatics()
    {
        canAttack = true;
        menuPaused = false;
        actionPaused = false;
        running = false;
    }

    public void ZoomIn()
    {
        if (currentZoom > 0)
            zoomer.SetDestination(cameraZooms[--currentZoom]);

        zoomButtons[1].interactable = true;
        RefreshZoomButtons();
    }

    public void ZoomOut()
    {
        if (currentZoom < cameraZooms.Count - 1)
            zoomer.SetDestination(cameraZooms[++currentZoom]);

        zoomButtons[0].interactable = true;
        RefreshZoomButtons();
    }

    public void RefreshZoomButtons()
    {
        if (currentZoom == 0)
            zoomButtons[0].interactable = false;

        if (currentZoom == cameraZooms.Count - 1)
            zoomButtons[1].interactable = false;
    }

    public void MenuPause()
    {
        menuPaused = !menuPaused;

        pauseMenu.SetActive(menuPaused);
        // SetState(menuPaused ? ActionManager.State.MenuPause : );
        foreach (Button button in allButtons)
            button.gameObject.SetActive(!menuPaused);
            // button.interactable = !menuPaused;

        actionButton.interactable = canAttack && !menuPaused;
        if (!menuPaused)
            RefreshZoomButtons();

        pauseDisplay.sprite = menuPaused ? sprites[0] : sprites[1];
        Time.timeScale = menuPaused ? 0 : 1;
    }

    // Pauses the player and all enemies (does not modify timeScale)
    public void ActionPause(bool paused)
    {
        actionPaused = paused;

        PlayerMover.instance.enabled = !paused;

        GameObject enemyList = EnemyList.instance.gameObject;

        foreach(Transform child in enemyList.transform)
        {
            AutoMover autoMover = child.GetComponent<AutoMover>();
            if (autoMover != null)
                autoMover.enabled = !paused;
        }

    }

    public void ToggleInGameDialogue(bool viewingDialogue)
    {
        ActionPause(viewingDialogue);
        dialogueSidebarContainer.SetActive(viewingDialogue);
        gameObject.SetActive(!viewingDialogue);
    }

    public static bool GetMenuPaused()
    {
        return menuPaused;
    }

    public static bool GetActionPaused()
    {
        return actionPaused;
    }

    public void ToggleRunning()
    {
        running = !running;
        Toast.ToastWrapper("Movement speed set to " + (running ? "run" : "walk"));
        RefreshRunning();
    }

    private void RefreshRunning()
    {
        foreach(Image icon in moveIcons)
            icon.sprite = running ? sprites[3] : sprites[2];
    }

    public static bool GetRunning()
    {
        return running;
    }

    public void SelectAction(string actionType)
    {
        ActionManager.SetSelectedAction(actionType);
    }

    public void ConfirmAim()
    {

    }

    public void StartAction()
    {
        foreach (Button button in moveConfirmButtons)
            button.interactable = false;
        
        ActionManager.ExecuteAction();
        SetState("Acting");
    }

    // Reset the menu options when an action finishes
    public void FinishAction()
    {
        menuNavigator.ShowMenu(0);
        currentCooldown = actionCooldown;
        SetState("Moving");
        ActionPause(false);

        foreach (Button button in moveConfirmButtons)
            button.interactable = true;
    }

    public void SetState(string state)
    {
        // State { Moving, Paused, ActionMenu, AimedConfirm, NonAimConfirm };
        ActionManager.SetState(state);
    }

}
