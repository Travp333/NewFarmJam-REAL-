using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class SeedGetter : MonoBehaviour
{
    [SerializeField]
	public Item[] seeds;

    void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.tag == "Player"){
        //    DropMe();
        //}
    }
	public void PickUpSeed(int odds){
		int rando = Random.Range(0, odds);
		if(rando == 4){
			Item randomSeed = seeds[Random.Range(0, seeds.Length)];
			GameObject.Find("3rd Person Character").GetComponent<Inven>().SmartPickUp(randomSeed);
		}

	}
    public void DropMe(){
            
        
        
    }
}
