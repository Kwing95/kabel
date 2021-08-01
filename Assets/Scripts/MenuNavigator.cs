using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{

    private List<GameObject> menus;
    private List<int> menuPath;

    // Start is called before the first frame update
    void Start()
    {
        menuPath = new List<int>();
        menus = new List<GameObject>();
        foreach (Transform child in transform)
            if (char.IsNumber(child.gameObject.name[0]))
                menus.Add(child.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuBack()
    {
        int prevMenu = menuPath.Count >= 2 ? menuPath[menuPath.Count - 2] : 0;
        if (menuPath.Count > 0)
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
        else if (menuPath.Count == 0 || menuPath[menuPath.Count - 1] != menuIndex)
            menuPath.Add(menuIndex);

        // Actually display menu
        for (int i = 0; i < menus.Count; ++i)
            menus[i].SetActive(i == menuIndex);
    }

}
