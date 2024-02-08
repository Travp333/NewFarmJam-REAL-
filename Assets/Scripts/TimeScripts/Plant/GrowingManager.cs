using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingManager : MonoBehaviour, SaveInterface
{
    List<Growable> plants;
    [SerializeField]
    GameClock GameClock;

    void Start()
    {
        if (plants == null)
        {
            plants = new List<Growable>();
        }
        ClockCheck();

    }
	private void Awake()
	{
        ClockCheck();
	}

	public void GrowStepUpdate() {
        foreach (Growable g in plants) 
        {
            g.UpdateGrowth();
            
        }
    }
    void ClockCheck() {
        if (GameClock = null) {
            GameClock = FindObjectOfType<GameClock>();
            if (GameClock = null) {
                Debug.Log("Clock is still null");
            }
        }
        GameClock.growingManager = this;
    }
    public void LoadData(SaveData s) {
        this.plants = s.plants;
    }
    public void SaveData(ref SaveData s)
    {
        s.plants = this.plants;
    }
    void AddCrop(Vector3 pos) 
    {
        GrowableCrop c = new GrowableCrop();
        c.position = pos;
        
        plants.Add(c);
        
    }
}
