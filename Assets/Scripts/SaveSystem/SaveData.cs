using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int gameHour;
    public float totalPlayTime;
    public List<Growable> plants;
    public SaveData(){
        this.gameHour = 0;
        this.totalPlayTime = 0f;
        this.plants = new List<Growable>();
    }
}
