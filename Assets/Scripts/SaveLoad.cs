using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    static public void Save(string path, GameController gameController, string key = "")
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
    static public void Load(string path, GameController gameController, string key = "")
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
                LoadNewGame(path, gameController, key);
            }
        }
        else
        {
            LoadNewGame(path, gameController, key);
        }
    }
    static public void LoadNewGame(string path, GameController gameController, string key = "")
    {
        Save(path, gameController, key);
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
