using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;

public class Menu : MonoBehaviour
{
    [SerializeField]
        GameObject MainMenu;
    [SerializeField]
        GameObject RankingMenu;

    GameObject currentMenu;

    public void NewGame()
    {
        File.Delete(Application.dataPath + SaveLoad.path);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Continue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Ranking()
    {
        currentMenu = RankingMenu;
        MainMenu.SetActive(false);
        RankingMenu.SetActive(true);
    }
    public void Escape(InputAction.CallbackContext contex)
    {
        if (contex.started)
            BackToMenu();
    }
    public void BackToMenu()
    {
        MainMenu.SetActive(true);
        currentMenu.SetActive(false);
    }
}
