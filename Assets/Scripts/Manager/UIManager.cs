using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    [SerializeField] List<GameObject> buildMenus = null;
    [SerializeField] GameObject buildMenu = null;
    [SerializeField] GameObject lobby = null;

    void Awake()
    {
        if (Instance != this)
            Instance = this;
    }

    public void ChangeMenu(int i)
    {
        GameFuncs.ChangeMenu(buildMenus.ToArray(), i);
    }

    public void ChangeBuildMenu()
    {
        buildMenu.SetActive(!buildMenu.activeInHierarchy);
        ChangeMouseUIState();
    }

    public void ChangeLobby()
    {
        lobby.SetActive(!lobby.activeInHierarchy);
        ChangeMouseUIState();
    }

    public void ChangeMouseUIState()
    {
        MouseManager.MouseState = MouseManager.MouseState == GameConsts.MOUSE_STATE.UI
            ? GameConsts.MOUSE_STATE.NONE
            : GameConsts.MOUSE_STATE.UI;
    }
}
