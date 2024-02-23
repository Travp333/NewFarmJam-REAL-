using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingManager : MonoBehaviour, SaveInterface
{
    List<Inven> plants;
    [SerializeField]
    GameClock GameClock;

    void Start()
    {
        if (plants == null)
        {
            plants = new List<Inven>();
        }
       
    }
	private void Awake()
	{
        
	}

    public void GrowStepUpdate()
    {
        plants.Clear();
        foreach (Inven i in GameObject.FindObjectsOfType<Inven>())
        {
            if (i.gameObject.tag != "Player")
            {
                plants.Add(i);
            }
        }
        foreach (Inven i in plants)
        {
            i.PlantAgeUpdate();
        }
    }
    
    
            
        
    
    public void LoadData(SaveData s) {
        //this.plants = s.plants;
    }
    public void SaveData(ref SaveData s)
    {
        //s.plants = this.plants;
    }
    
}
