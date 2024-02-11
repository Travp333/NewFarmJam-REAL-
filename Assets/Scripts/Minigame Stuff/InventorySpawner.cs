using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpawner : MonoBehaviour
{
	[SerializeField]
	GameObject UIPrefab;
	
	GameObject Player;
	[SerializeField]
	public tempHolder temp;
	
    // Start is called before the first frame update
    void Start()
	{
		foreach(Inven i in GameObject.FindObjectsOfType<Inven>()){
			if(i.gameObject.tag == "Player"){
				Player = i.gameObject;	
			}
		}
		foreach(Inven i in GameObject.FindObjectsOfType<Inven>()){
			if(i.gameObject.tag != "Player"){
				GameObject g = Instantiate(UIPrefab, this.transform);
				//Debug.Log("Pluggin Ui Plugger");
				i.UIPlugger = g.gameObject;
				//this needs to run right before line 98 of Inven
				g.GetComponent<UiPlugger>().inven = i;
				g.GetComponent<UiPlugger>().sync = i.GetComponent<InvenSyncer>();
				i.jumpStart();
				g.name = i.gameObject.name + " Inventory";
				Player.GetComponent<Interact>().HideAllInventories();
				
			}
		}
		temp.LateStart();
    }
}
