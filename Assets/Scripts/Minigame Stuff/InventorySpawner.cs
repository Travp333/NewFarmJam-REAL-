using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySpawner : MonoBehaviour
{
	[SerializeField]
	GameObject CookerUIPrefab;
	[SerializeField]
	GameObject PlantableUIPrefab;
	[SerializeField]
	GameObject UIPrefab;
	GameObject Player;
	[SerializeField]
	public tempHolder temp;
	/*[SerializeField]
	GrowingManager growingManager;*/
	
    // Start is called before the first frame update
    void Start()
	{
		foreach(Inven i in GameObject.FindObjectsOfType<Inven>()){
			if(i.gameObject.tag == "Player"){
				Player = i.gameObject;	
				i.jumpStart();
			}
		}
		foreach(Inven i in GameObject.FindObjectsOfType<Inven>()){
			if(i.gameObject.tag == "Plantable"){
				//Debug.Log("SPawning plantable inven");
				GameObject g = Instantiate(PlantableUIPrefab, this.transform);
				//Debug.Log("Pluggin Ui Plugger");
				i.UIPlugger = g.gameObject;
				g.GetComponent<UiPlugger>().inven = i;
				g.GetComponent<UiPlugger>().sync = i.GetComponent<InvenSyncer>();
				i.jumpStart();
				g.name = i.gameObject.name + " Inventory";
				Player.GetComponent<Interact>().HideAllInventories();
				
			}
			else if(i.gameObject.tag == "Cooker"){
				//Debug.Log("SPawning plantable inven");
				GameObject g = Instantiate(CookerUIPrefab, this.transform);
				//Debug.Log("Pluggin Ui Plugger");
				i.UIPlugger = g.gameObject;
				g.GetComponent<UiPlugger>().inven = i;
				g.GetComponent<UiPlugger>().sync = i.GetComponent<InvenSyncer>();
				i.jumpStart();
				g.name = i.gameObject.name + " Inventory";
				Player.GetComponent<Interact>().HideAllInventories();
				
			}
			else if(i.gameObject.tag != "Player"){
				//Debug.Log("SPawning regular inven");
				GameObject g = Instantiate(UIPrefab, this.transform);
				//Debug.Log("Pluggin Ui Plugger");
				i.UIPlugger = g.gameObject;
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
