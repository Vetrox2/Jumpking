using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject MainMenu, RankingMenu;
    [SerializeField]
    ScoreTable[] scoreTable;
    [SerializeField]
    Button WorldHighscoreButton, UserHighscoreButton;

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
        RealmController<Highscore> realmController = new();
        realmController.RealmLoaded += GetHighscore;
        realmController.InitAsync();
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
    
    void GetHighscore(RealmController<Highscore> realmController)
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
    void UpdateRanking()
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
                for (int i = 0; i < scoreTable.Length && i < worldHighscores.Count; i++)
                {
                    scoreTable[i].name.text = worldHighscores[i].Player;
                    scoreTable[i].time.text = worldHighscores[i].Time.ToString();
                }
                break;
            case HighscoreType.UserHighscore:
                UserHighscoreButton.interactable = false;
                WorldHighscoreButton.interactable = true;
                for (int i = 0; i < scoreTable.Length && i < userHighscores.Count; i++)
                {
                    scoreTable[i].name.text = userHighscores[i].Player;
                    scoreTable[i].time.text = userHighscores[i].Time.ToString();
                }
                break;
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
public enum HighscoreType
{
    WorldHighscore,
    UserHighscore
}
