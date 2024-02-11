﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script creates a reference to the storage object and calls methods on it. 
//This was a workaround to allow calling methods with arguments through the unity event system.
//Written by Travis
public class StorageFinder : MonoBehaviour
{
	public GameObject storage;
	public Inven storageInven;
	GameObject player;
	
	tempHolder tH;
	void Start()
	{
		//Debug.Log("STORAGE FINDER HAS AWOKEN");
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")){
			if(g.GetComponent<tempHolder>() != null){
				player = g;
			}
		}
		if(this.transform.parent.parent.parent.GetComponent<UiPlugger>() != null){
			storage = this.transform.parent.parent.parent.GetComponent<UiPlugger>().inven.gameObject;
		}
		if(storage.GetComponent<Inven>() != null){
			storageInven = storage.GetComponent<Inven>();
		}
		if(player.GetComponent<tempHolder>() != null){
			tH = player.GetComponent<tempHolder>();
		}
		
	}
	public void SendDropItem(){
		storageInven.DropSpecificItem(this.gameObject.transform.parent.name);
	}
	public void SendDropAllItems(){
		//Debug.Log("MAde it to storage finder");
		storageInven.DropWholeStack(this.gameObject.transform.parent.name);
		tH.ClearSlot();
	}
	//public void SendSwap() {	 
		//	tH.Swap(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
		//}
	public void SendPickUp(){
		tH.HoldItem(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
	}
	public void SendReleaseItem(){
		Debug.Log("Made it to release Item in storage Finder, passing " + storage.GetComponent<Inven>().name);
		tH.DropItem(storage.GetComponent<Inven>(), this.gameObject.transform.parent.name);
	}
}
