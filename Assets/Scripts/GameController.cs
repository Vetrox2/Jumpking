using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static string key = "4163731634";
    [HideInInspector]
    public float time = 0;
    string path = "/Saves/save.sav";
    private void Start()
    {
        SaveLoad.Load(path, this, key);
        StartCoroutine(ActiveSaving());
    }
    private void Update()
    {
        time += Time.deltaTime;
    }
    public void Finished()
    {
        //koniec, wyslanie wynikow itp
    }
    IEnumerator ActiveSaving()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            SaveLoad.Save(path, this, key);
        }
    }
    static public GameObject FindPlayer()
    {
        GameObject player;
        if (player = GameObject.FindGameObjectWithTag("Player"))
            return player;
        return null;
    }
}
