using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sidebar : MonoBehaviour
{

    public static Sidebar instance;

    public TextMeshProUGUI toast;
    public List<GameObject> menus;
    public List<Sprite> sprites;

    public float actionCooldown = 3;

    public enum Menu { Main, Action, Grenade, Confirm };
    private List<int> menuPath;

    public List<Button> allButtons;
    public List<Button> actionConfirmButtons;
    public List<Button> moveConfirmButtons;
    public List<Button> zoomButtons;
    public Button actionButton;

    public List<Image> moveIcons;
    public Image pauseDisplay;

    public List<int> cameraZooms;
    private int currentZoom = 0;

    private static int toastNonce = 0;
    private static bool canAttack = true;
    private static bool menuPaused = false;
    private static bool actionPaused = false;
    private static bool running = false;

    private Zoomer zoomer;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        menuPath = new List<int>();
        zoomer = Camera.main.GetComponent<Zoomer>();
        currentZoom = 0;
        ZoomOut();
        RefreshRunning();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    // Disable the action button for a bit following an action
    public IEnumerator DisableActions()
    {
        actionButton.interactable = canAttack = false;
        yield return new WaitForSeconds(actionCooldown);
        actionButton.interactable = canAttack = true;
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
        StartCoroutine(Toast("Movement speed set to " + (running ? "run" : "walk")));
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

    public void MenuBack()
    {
        int prevMenu = menuPath.Count >= 2 ? menuPath[menuPath.Count - 2] : 0;
        if(menuPath.Count > 0)
            menuPath.RemoveAt(menuPath.Count - 1);
        ShowMenu(prevMenu);
    }

    // Shows the menu at menuIndex
    public void ShowMenu(int menuIndex)
    {
        // If navigating to root (0) menu, delete menuPath
        if (menuIndex == 0)
            menuPath.Clear();
        
        // Don't re-add the same item
        else if(menuPath.Count == 0 || menuPath[menuPath.Count - 1] != menuIndex)
            menuPath.Add(menuIndex);

        // Actually display menu
        for(int i = 0; i < menus.Count; ++i)
            menus[i].SetActive(i == menuIndex);
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
        ShowMenu(0);
        StartCoroutine(DisableActions());
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

    public void ToastWrapper(string message)
    {
        StartCoroutine(Toast(message));
    }

    public IEnumerator Toast(string message, float duration=3)
    {
        int currentToast = ++toastNonce;
        toast.text = message;

        yield return new WaitForSeconds(duration);

        if(currentToast == toastNonce)
            toast.text = "";
    }

}
