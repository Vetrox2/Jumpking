using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    static string key = "4163731634";
    static public string path = "/Saves/save.sav";
    static public void Save(GameController gameController)
    {
        var player = GameController.FindPlayer();
        SaveData save = new SaveData
        {
            time = gameController.time,
            playerPos = player.transform.position,
            playerVelocity = player.GetComponent<Rigidbody2D>().velocity,
            playerDuringJump = player.GetComponent<PlayerController>().duringJump
        };
        string saveStr = JsonUtility.ToJson(save);
        saveStr = EncryptFile(saveStr, key);
        File.WriteAllText(Application.dataPath + path, saveStr);
    }
    static public void Load(GameController gameController)
    {
        var player = GameController.FindPlayer();
        if (File.Exists(Application.dataPath + path))
        {
            string saveStr = File.ReadAllText(Application.dataPath + path);
            saveStr = EncryptFile(saveStr, key);
            try
            {
                var save = JsonUtility.FromJson<SaveData>(saveStr);
                gameController.time = save.time;
                player.transform.position = save.playerPos;
                player.GetComponent<Rigidbody2D>().velocity = save.playerVelocity;
                if (save.playerDuringJump)
                    player.GetComponent<PlayerController>().LoadDuringJump();
            }
            catch
            {
                File.Delete(Application.dataPath + path);
                LoadNewGame(gameController);
            }
        }
        else
        {
            LoadNewGame(gameController);
        }
    }
    static public void LoadNewGame(GameController gameController)
    {
        Save(gameController);
    }
    static string EncryptFile(string file, string key)
    {
        string result = "";
        for (int i = 0; i < file.Length; i++)
        {
            result += (char)(file[i] ^ key[i % key.Length]);
        }
        return result;
    }
    
}
