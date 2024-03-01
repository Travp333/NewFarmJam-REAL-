using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingManager : MonoBehaviour
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
            if (i.gameObject.CompareTag("Plantable") && !plants.Contains(i))
            {
                plants.Add(i);
            }
        }
        foreach (Inven i in plants)
        {
            i.PlantAgeUpdate();
        }
    }
    
    
            
        
    
    
}
