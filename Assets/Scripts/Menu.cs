using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;
using System;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject MainMenu;
    [SerializeField]
    GameObject RankingMenu;
    [SerializeField]
    GameObject SetNameScreen;
    [SerializeField]
    TMP_InputField SetNameField;
    [SerializeField]
    ScoreTable[] scoreTable;

    GameObject currentMenu;

    private void Start()
    {
        if (PlayerPrefs.GetString("name") == null || PlayerPrefs.GetString("name") == "")
        {
            SetNameScreen.SetActive(true);
            currentMenu = SetNameScreen;
        }
    }
    public void SetName()
    {
        PlayerPrefs.SetString("name", SetNameField.text);
        BackToMenu();
    }
    public void NewGame()
    {
        SaveLoad.DeleteSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Continue() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public void Exit() => Application.Quit();

    public void Ranking()
    {
        currentMenu = RankingMenu;
        MainMenu.SetActive(false);
        RankingMenu.SetActive(true);
        RealmController realmController = new();
        var scores = realmController.GetHighscore();
        for (int i = 0; i < scoreTable.Length && i < scores.Count; i++)
        {
            scoreTable[i].name.text = scores[i].Player;
            scoreTable[i].time.text = scores[i].Time.ToString();
        }
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
