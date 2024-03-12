using System.IO;
using UnityEngine;

public static class SaveLoad
{
    static string key = "4163731634";

    public static string path { get; private set; } = "/save.sav";


    public static void Save(GameController gameController)
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

    public static void Load(GameController gameController)
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
                DeleteSave();
                LoadNewGame(gameController);
            }
        }
        else
        {
            LoadNewGame(gameController);
        }
    }

    public static void LoadNewGame(GameController gameController)
    {
        Save(gameController);
    }

    public static void DeleteSave()
    {
        if (File.Exists(Application.dataPath + path))
            File.Delete(Application.dataPath + path);
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
