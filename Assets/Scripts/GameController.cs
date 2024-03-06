using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public float time = 0;
    private void Start()
    {
        SaveLoad.Load(this);
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
            SaveLoad.Save(this);
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
