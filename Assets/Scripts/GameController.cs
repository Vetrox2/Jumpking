using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Camera Camera;
    public GameObject EndingScreen;
    public GameObject Menu;
    [HideInInspector]
    public float time = 0;
    
    bool finished = false;
    private void Start()
    {
        SaveLoad.Load(this);
        StartCoroutine(ActiveSaving());
        Camera.GetComponent<CameraController>().InitializeCameraPosition();
        
    }
    private void Update()
    {
        time += Time.deltaTime;
    }
    public void Finished()
    {
        if (!finished)
        {
            finished = true;
            EndingScreen.SetActive(true);
            RealmController realmController = new();
            realmController.SendHighscore(PlayerPrefs.GetString("name"), time);
            StopAllCoroutines();
            Invoke("DeleteSave", 0.3f);
        }
    }
    void DeleteSave()
    {
        SaveLoad.DeleteSave();
    }
    IEnumerator ActiveSaving()
    {
        while (!finished)
        {
            SaveLoad.Save(this);
            yield return new WaitForSeconds(0.2f);
        }
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
        SceneManager.LoadScene(0);
    }
    public void Escape()
    {
        Menu.SetActive(!Menu.active);
    }
}
