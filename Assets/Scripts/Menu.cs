using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu, RankingMenu;
    [SerializeField]
    private ScoreTable[] scoreTable;
    [SerializeField]
    private Button WorldHighscoreButton, UserHighscoreButton;

    private GameObject currentMenu;
    private List<Highscore> userHighscores, worldHighscores;
    private HighscoreType activeHighscoreType;

    private void Start()
    {
        activeHighscoreType = HighscoreType.WorldHighscore;
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
        RealmController<Highscore> realmController = new(GetHighscore);
    }

    public void ChangeRankingView(int index)
    {
        switch (index)
        {
            case 0:
                activeHighscoreType = HighscoreType.WorldHighscore;
                break;
            case 1:
                activeHighscoreType = HighscoreType.UserHighscore;
                break;
        }
        UpdateRanking();
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

    private void GetHighscore(RealmController<Highscore> realmController)
    {
        var scores = realmController.GetHighscore();
        worldHighscores = scores.GroupBy(score => score.Player)
                        .Select(group => new Highscore
                        {
                            Player = group.Key,
                            Time = group.Min(score => score.Time)
                        }).OrderBy(player => player.Time).Take(scoreTable.Length).ToList();

        userHighscores = scores.Where(score => score.Player == PlayerPrefs.GetString("UserName"))
                        .OrderBy(player => player.Time).Take(scoreTable.Length).ToList();
        UpdateRanking();
    }

    private void UpdateRanking()
    {
        for (int i = 0; i < scoreTable.Length; i++)
        {
            scoreTable[i].name.text = string.Empty;
            scoreTable[i].time.text = string.Empty;
        }
        switch (activeHighscoreType)
        {
            case HighscoreType.WorldHighscore:
                UserHighscoreButton.interactable = true;
                WorldHighscoreButton.interactable = false;
                SetScoreTable(worldHighscores);
                break;
            case HighscoreType.UserHighscore:
                UserHighscoreButton.interactable = false;
                WorldHighscoreButton.interactable = true;
                SetScoreTable(userHighscores);
                break;
        }
    }

    private void SetScoreTable(List<Highscore> highscores)
    {
        for (int i = 0; i < scoreTable.Length && i < highscores.Count; i++)
        {
            scoreTable[i].name.text = highscores[i].Player;
            scoreTable[i].time.text = highscores[i].Time.ToString();
        }
    }
}

