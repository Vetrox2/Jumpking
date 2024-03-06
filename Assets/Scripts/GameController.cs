using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static string key = "4163731634";
    float time = 0;
    string path = "/Saves/save.sav";
    private void Start()
    {
        Load();
        StartCoroutine(ActiveSaving());
    }
    private void Update()
    {
        time += Time.deltaTime;
        Debug.Log(time);
    }
    public void Finished()
    {
        //koniec, wyslanie wynikow itp
    }

    void Save()
    {
        var player = FindPlayer();
        SaveData save = new SaveData { time = time, playerPos = player.transform.position, 
            playerVelocity = player.GetComponent<Rigidbody2D>().velocity,
            playerDuringJump = player.GetComponent<PlayerController>().duringJump };
        string saveStr = JsonUtility.ToJson(save);
        saveStr = EncryptFile(saveStr);
        File.WriteAllText(Application.dataPath + path, saveStr);
    }
    void Load()
    {
        var player = FindPlayer();
        if (File.Exists(Application.dataPath + path))
        {
            string saveStr = File.ReadAllText(Application.dataPath + path);
            saveStr = EncryptFile(saveStr);
            try
            {
                var save = JsonUtility.FromJson<SaveData>(saveStr);
                time = save.time;
                player.transform.position = save.playerPos;
                player.GetComponent<Rigidbody2D>().velocity = save.playerVelocity;
                if (save.playerDuringJump)
                    player.GetComponent<PlayerController>().LoadDuringJump();
            }
            catch
            {
                File.Delete(Application.dataPath + path);
                LoadNewGame();
            }
        }
        else
        {
            LoadNewGame();
        }
    }
    void LoadNewGame()
    {
        Save();
    }
    static public GameObject FindPlayer()
    {
        GameObject player;
        if (player = GameObject.FindGameObjectWithTag("Player"))
            return player;
        return null;
    }

    static public string EncryptFile(string file)
    {
        string result = "";
        for (int i = 0; i < file.Length; i++)
        {
            result += (char)(file[i] ^ key[i % key.Length]);
        }
        return result;
    }
    IEnumerator ActiveSaving()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            Save();
        }
    }
    public struct SaveData
    {
        public float time;
        public Vector2 playerPos;
        public Vector2 playerVelocity;
        public bool playerDuringJump;
    }
}
