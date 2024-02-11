using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script handles the Swap mechanic in the inventory. it has a tempslot that we store inventory objects we are looking to swap in,
//and also handles the logic of doing the actual swap.
//written by Conor and Travis
public class tempHolder : MonoBehaviour
{
	[SerializeField]
	GameObject emptyModel;
	[HideInInspector]
	public ItemStat slot = null;
	[HideInInspector]
	public int tempRow = -1, tempColumn = -1, tempCount = -1;
	[HideInInspector]
	public string tempName = null;
	Sprite tempImage = null;
	public GameObject tempModel = null;
	public Sprite emptyImage;
	Inven tempInven = null;
	UiPlugger tempPlug;
	Inven playerInven;
	Inven openStorageInven;
	public int slotAmount;
	

	private void Start()
	{
		//make sure nothing is in the slot, set all the values to defaults
		ClearSlot();
		//find every UI plugger component, and call the proper methods. in the player's case,
		//we also store a reference to it here since it is static
		if (GameObject.Find("3rd Person Character").GetComponent<Inven>() != null){
			//Debug.Log("Found player Inven!");
			Inven playerInven = GameObject.Find("3rd Person Character").GetComponent<Inven>();
			playerInven.UIPlugger.GetComponent<UiPlugger>().SpawnButtonsPlayer();
		}
		//Invoke("LateStart", .01f);
	}
	public void LateStart(){
		foreach(UiPlugger i in GameObject.FindObjectsOfType<UiPlugger>()){
			//Debug.Log(i.name);
			if(i.gameObject.tag != "Player"){
				//Debug.Log("SPAWNING BUTTONS");
				i.SpawnButtonsStorage();
			}
		}
	}
	//resets values to defaults, ensures the slot is empty.
	public void ClearSlot(){
		foreach(UiPlugger i in GameObject.FindObjectsOfType<UiPlugger>()){
			if(i.inven == tempInven){
				i.ButtonDeselected(tempRow, tempColumn);
			}
		}
		slot = null;
		tempRow = -1;
		tempColumn = -1;
		tempCount = -1;
		tempName = null;
		tempImage = emptyImage;
		tempInven = null;
	}
	public void HoldItem(Inven inventoryObject, string coords){
		//Debug.Log("running HoldItem()");
		//store a reference to the Ui script, as we will use it often
		UiPlugger plug = inventoryObject.UIPlugger.GetComponent<UiPlugger>();
		//gets reference to uiplugger component on the item stored in the slot, if there is one
		if(tempInven != null){
			tempPlug = tempInven.UIPlugger.GetComponent<UiPlugger>();
		}
		//parse the string into two ints, the string will be coordinates, structured like (1,2), which will then be parsed into 1 and 2. 
		string[] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		// here, slot refers to the temporary slot in which you are holding an item you selected which you want to swap
		// if it is null, we know it it empty, therefore we just need to store the clicked item to the slot 
		if (slot == null) {
			//check if the player is holding shift, behaviour changes in that case
			//NOTE make sure to tie this shift to the input system
			//find out what inventory slot the coordiantes you were passed points to, and store that data in the temp slot
			slot = inventoryObject.array[row,column];
			//if that data is named "", we know it is empty, and therefore we do not need to store it in the temp slot. 
			if(slot.Name != ""){
				//if the name is anything else, we know it is a valid inventory object, so we store its data in the temp slot as well as info needed for the UI
				tempRow = row; 
				tempColumn = column;
				tempName = slot.Name;
				tempImage = slot.image;
				tempCount = slot.Amount;
				tempInven = inventoryObject;
				//Debug.Log(slot.Name + " was selected");
				//This turns the button pressed darker, to indicate to the player that that inventory slot is being stored in the temp slot
				//plug.ButtonSelected(row, column);	
			}
			else{
				//if the slot we picked is empty, we dont need to store any info on it and can jsut clear it out
				slot = null;
			}
		}
	}
	//second half of Swap
	public void DropItem(Inven inventoryObject, string coords){
		//Debug.Log("Made it to drop item in tempHolder");
		//store a reference to the Ui script, as we will use it often
		UiPlugger plug = inventoryObject.UIPlugger.GetComponent<UiPlugger>();
		//gets reference to uiplugger component on the item stored in the slot, if there is one
		if(tempInven != null){
			tempPlug = tempInven.UIPlugger.GetComponent<UiPlugger>();
		}
		//parse the string into two ints, the string will be coordinates, structured like (1,2), which will then be parsed into 1 and 2. 
		string[] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		// here, slot refers to the temporary slot in which you are holding an item you selected which you want to swap
		// if it is null, we know it it empty, therefore we just need to store the clicked item to the slot 
		
		if (slot != null) {
			//Debug.Log("The slot was not null in tempHolder, here data " + slot + ", " + slot.Name);
			//if the temp slot is not null, we know it is holding a valid inventory object. So, we must initiate the swap
			//if we are swapping two objects with the same name, prepare to stack!
			if(tempInven.array[tempRow, tempColumn].Name == inventoryObject.array[row, column].Name) {
				if(row == tempRow && tempColumn == column){
					//Debug.Log("same object, doing nothing. heres some data " + inventoryObject.array[row, column].Name + ", " + row + ", " + column);
					//same name, same slot, same object, do nothing, reset
					ClearSlot();
				}
				else{
					//Debug.Log("Different slots, same name, merging");
					//two different slots, but same name. merge stacks
					//check if you can just call them and keep it under that item's stack size
					if((tempInven.array[tempRow, tempColumn].Amount + inventoryObject.array[row, column].Amount) > inventoryObject.array[row, column].StackSize){
						//we cant do that, set the second buttons count to the max and subtract the necessary amount from the first button's amount
						tempInven.array[tempRow, tempColumn].Amount = ((inventoryObject.array[row, column].Amount + tempInven.array[tempRow, tempColumn].Amount) - inventoryObject.array[row, column].StackSize);
						//updateUI
						tempPlug.UpdateItem(tempRow, tempColumn, tempInven.array[tempRow, tempColumn].Amount);
						inventoryObject.array[row, column].Amount = inventoryObject.array[row, column].StackSize;
						//update UI
						plug.UpdateItem(row, column, inventoryObject.array[row, column].Amount);
						ClearSlot();
						
					}
					else{
						//Debug.Log("Stacking two stacks of same item type");
						//we can simply add the temp slot and second button press together
						//add the items in temp slot to the second pressed button's slot, clear out original button's slot and temp slot
						inventoryObject.array[row, column].Amount = tempInven.array[tempRow, tempColumn].Amount + inventoryObject.array[row, column].Amount;
						plug.UpdateItem(row, column, inventoryObject.array[row, column].Amount);
						tempInven.array[tempRow, tempColumn].Name = "";
						tempInven.array[tempRow, tempColumn].Amount = 0;
						tempInven.array[tempRow, tempColumn].image = emptyImage;
						//tempPlug.SyncWorldModel(tempRow,tempColumn, "", emptyModel); 
						tempPlug.ChangeItem(tempRow,tempColumn, emptyImage, 0, "");
						ClearSlot();
					}
				}
			}
			else{
				//GameObject g2 = inventoryObject.array[row, column].worldModel[Random.Range(0, inventoryObject.array[row, column].worldModel.Length-1)];
				//Debug.Log("Clean swap, two different objects, doing swap. Object 1 is "+ tempInven.array[tempRow, tempColumn].image.name + " and Object 2 is " + inventoryObject.array[row, column].image.name + " and finally, this is Slot: "+ slot.Name);
				//clean swap, two different objects
				//we find the inventory slot the tempslot object is pointing to, and set it equal to the second button's data
				tempInven.array[tempRow, tempColumn] = inventoryObject.array[row, column];
				//we then update the Ui to follow suit
				//tempPlug.SyncWorldModel(tempRow,tempColumn, inventoryObject.array[row, column].Name, g2); 
				tempPlug.ChangeItem(tempRow, tempColumn, inventoryObject.array[row, column].image, inventoryObject.array[row, column].Amount, inventoryObject.array[row, column].Name);
				//then we set the second button equal to the temp slot's data
				inventoryObject.array[row, column] = slot;
				//we also have the Ui update
				plug.SyncWorldModel(row,column, tempName, emptyModel);
				plug.ChangeItem(row,column, tempImage, tempCount, tempName);
				ClearSlot();		
			}
		}
	}
}
