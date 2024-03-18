using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public float time = 0;

    [SerializeField]
    private Camera Camera;
    [SerializeField]
    private GameObject EndingScreen;
    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private TextMeshProUGUI TimeText;

    private bool finished;

    private void Start()
    {
        SaveLoad.Load(this);
        StartCoroutine(ActiveSaving());
        Camera.GetComponent<CameraController>().InitializeCameraPosition();
        if (!Menu.activeInHierarchy)
            Cursor.visible = false;
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    static public GameObject FindPlayer()
    {
        GameObject player;
        if (player = GameObject.FindGameObjectWithTag("Player"))
            return player;
        return null;
    }

    public void BackToMenu()
    {
        if (!finished)
            SaveLoad.Save(this);
        SceneManager.LoadScene(1);
    }

    public void Escape()
    {
        if (!finished)
        {
            Menu.SetActive(!Menu.activeInHierarchy);
            Cursor.visible = Menu.activeInHierarchy;
        }
    }

    public void Finished()
    {
        if (!finished)
        {
            finished = true;
            TimeText.text = "Your time: " + System.Math.Round(time, 2).ToString();
            EndingScreen.SetActive(true);

            RealmController<Highscore> realmController = new(SendHighscoreToDB);

            StopAllCoroutines();
            Invoke("DeleteSave", 0.3f);
            Cursor.visible = true;
        }
    }

    private void SendHighscoreToDB(RealmController<Highscore> realmController) => realmController.SendHighscore(PlayerPrefs.GetString("UserName"), time);

    private void DeleteSave() => SaveLoad.DeleteSave();

    private IEnumerator ActiveSaving()
    {
        while (!finished)
        {
            SaveLoad.Save(this);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
