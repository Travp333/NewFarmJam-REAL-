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
        

    }
	private void Awake()
	{
        
	}

	public void GrowStepUpdate() {
        foreach (Growable g in plants) 
        {
            g.UpdateGrowth();
            
        }
    }
    
    public void LoadData(SaveData s) {
        this.plants = s.plants;
    }
    public void SaveData(ref SaveData s)
    {
        s.plants = this.plants;
    }
    
}
