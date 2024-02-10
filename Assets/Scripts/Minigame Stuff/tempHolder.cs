﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script handles the Swap mechanic in the inventory. it has a tempslot that we store inventory objects we are looking to swap in,
//and also handles the logic of doing the actual swap.
//written by Conor and Travis
public class tempHolder : MonoBehaviour
{
	[HideInInspector]
	public ItemStat slot = null;
	[HideInInspector]
	public int tempRow = -1, tempColumn = -1, tempCount = -1;
	[HideInInspector]
	public string tempName = null;
	Sprite tempImage = null;
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
			Debug.Log("Found player Inven!");
			Inven playerInven = GameObject.Find("3rd Person Character").GetComponent<Inven>();
			playerInven.UIPlugger.GetComponent<UiPlugger>().SpawnButtonsPlayer();
		}
		//Invoke("LateStart", .01f);
	}
	public void LateStart(){
		foreach(UiPlugger i in GameObject.FindObjectsOfType<UiPlugger>()){
			Debug.Log(i.name);
			if(i.gameObject.tag != "Player"){
				Debug.Log("SPAWNING BUTTONS");
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
	public void ShiftClickCheck(Inven inventoryObject, string coords){
		//Debug.Log("Shift click check #2");
		if(!Input.GetKey(KeyCode.LeftShift)){
			//player not holding shift
		}
		//player was holding shift, do auto transfer of full stack
		else{
			UiPlugger plug = inventoryObject.UIPlugger.GetComponent<UiPlugger>();
			string[] coords2 = coords.Split(",");
			int row = int.Parse(coords2[0]);
			int column = int.Parse(coords2[1]);
			//Debug.Log("Shift click check #3");
			//is the given inventory object a player inventory?
			//if no, its a storage inventory
			if(inventoryObject.gameObject.tag != "Player"){
				//Call the shift click method, which handles moving the requested stack of items 
				ShiftClick(inventoryObject, playerInven, row, column, plug);
			}
			//in this case, the player is shift clicking from within their own inventory. We should do the same 
			//procedure here, just flipped. 
			else{
				//make sure the player has an interact component(they always will)
				if(playerInven.GetComponent<Interact>()!=null){
					//check if the player has a storage inventory open currently. 
					if(playerInven.GetComponent<Interact>().storageInvOpen){
						//search through all the storage devices in the scene, and find the one that is enabled. Ideally there is just
						//one enabled, in which case that is the one the player is currently interacting with
						foreach(GameObject i in  playerInven.GetComponent<Interact>().StorageInvenUI){
							//is is enabled? is it a player?
							if(i.activeInHierarchy && i.transform.parent.gameObject.tag != "Player"){
								//if it is enabled and is not a player, store a reference
								openStorageInven = i.transform.parent.GetComponent<UiPlugger>().inven;
							}
						}
						ShiftClick(inventoryObject, openStorageInven, row, column, plug);
					}
				}
			}
				
		}
	}
	//this method handles shift clicking a stack to instantly move it either from the player inventory to the storage, or vice versa. 
	public void ShiftClick(Inven invenObj, Inven otherInven, int row, int column, UiPlugger plug){
		//Debug.Log("Shift click check #4");
		//store slotAmount into variable to ensure for loop runs proper amount of times,
		//as the amount will be edited throughout the for loop
		slotAmount = invenObj.array[row, column].Amount;
		//for loop to ensure every single item in the stack is moved
		//Debug.Log("This will run "+ slotAmount + " times");
		for (int i = 0; i < slotAmount; i++) {
			//possibly unnecessary since the loop will already run as any times as the amount
			if(invenObj.array[row, column].Amount > 0){	
				//calls the smartpickup method on the player inventory, since we know it is on a storage inventory
				//object. This will call the overload that uses itemstat instead of item. 
				otherInven.SmartPickUp(invenObj.array[row, column]);
				//this bool will be checked true in smartpickup if the pickup was successful
				if(otherInven.isPickedUp){
					//the pickup was successfull, so reflect that on this side. 
					invenObj.array[row, column].Amount = invenObj.array[row, column].Amount - 1;
					//update Ui to match
					plug.UpdateItem(row, column, invenObj.array[row, column].Amount);
					//check if that was the last one. if this slot is now empty, we must update that as well
					if(invenObj.array[row, column].Amount <= 0){
						invenObj.array[row, column].Name = "";
						invenObj.array[row, column].Amount = 0;
						invenObj.array[row, column].image = emptyImage;
						//invenObj.array[row, column].full = false;
						//update UI
						plug.ChangeItem(row, column, emptyImage, 0, "");
					}
					//set it back to false now that the needed procedures have been done
					otherInven.isPickedUp = false;
					//Debug.Log("ispickedup set to "+ otherInven.isPickedUp);
				}
				//if ipickedup is false, then the pickup failed, likely due to the inventory being full.
				//in this case, we do nothing. 
				else{
					Debug.Log("Full Inventory");
				}
			}
			else{
				//Debug.Log("Nothing here!");
			}
		}
	}
	//this method handles the logic of deciding whether to store somethign into the slot, or to empty the slot into a given inventory slot
	//public void Swap(Inven inventoryObject, string coords) {
	//	//store a reference to the Ui script, as we will use it often
	//	UiPlugger plug = inventoryObject.UIPlugger.GetComponent<UiPlugger>();
	//	//gets reference to uiplugger component on the item stored in the slot, if there is one
	//	if(tempInven != null){
	//		tempPlug = tempInven.UIPlugger.GetComponent<UiPlugger>();
	//	}
	//	//parse the string into two ints, the string will be coordinates, structured like (1,2), which will then be parsed into 1 and 2. 
	//	string[] coords2 = coords.Split(",");
	//	int row = int.Parse(coords2[0]);
	//	int column = int.Parse(coords2[1]);
	//	// here, slot refers to the temporary slot in which you are holding an item you selected which you want to swap
	//	// if it is null, we know it it empty, therefore we just need to store the clicked item to the slot 
	//	if (slot == null) {
	//		//check if the player is holding shift, behaviour changes in that case
	//		//NOTE make sure to tie this shift to the input system
	//		if(!Input.GetKey(KeyCode.LeftShift)){
	//			//find out what inventory slot the coordiantes you were passed points to, and store that data in the temp slot
	//			slot = inventoryObject.array[row,column];
	//			//if that data is named "", we know it is empty, and therefore we do not need to store it in the temp slot. 
	//			if(slot.Name != ""){
	//				//if the name is anything else, we know it is a valid inventory object, so we store its data in the temp slot as well as info needed for the UI
	//				tempRow = row; 
	//				tempColumn = column;
	//				tempName = slot.Name;
	//				tempImage = slot.image;
	//				tempCount = slot.Amount;
	//				tempInven = inventoryObject;
	//				//Debug.Log(slot.Name + " was selected");
	//				//This turns the button pressed darker, to indicate to the player that that inventory slot is being stored in the temp slot
	//				plug.ButtonSelected(row, column);	
	//			}
	//			else{
	//				//if the slot we picked is empty, we dont need to store any info on it and can jsut clear it out
	//				slot = null;
	//			}
	//		}
	//		//player was holding shift, do auto transfer of full stack
	//		else{
	//			//is the given inventory object a player inventory?
	//			//if no, its a storage inventory
	//			if(inventoryObject.gameObject.tag != "Player"){
	//				//Call the shift click method, which handles moving the requested stack of items 
	//				ShiftClick(inventoryObject, playerInven, row, column, plug);
	//			}
	//			//in this case, the player is shift clicking from within their own inventory. We should do the same 
	//			//procedure here, just flipped. 
	//			else{
	//				//make sure the player has an interact component(they always will)
	//				if(playerInven.GetComponent<Interact>()!=null){
	//					//check if the player has a storage inventory open currently. 
	//					if(playerInven.GetComponent<Interact>().storageInvOpen){
	//						//search through all the storage devices in the scene, and find the one that is enabled. Ideally there is just
	//						//one enabled, in which case that is the one the player is currently interacting with
	//						foreach(GameObject i in  playerInven.GetComponent<Interact>().StorageInvenUI){
	//							//is is enabled? is it a player?
	//							if(i.activeInHierarchy && i.transform.parent.gameObject.tag != "Player"){
	//								//if it is enabled and is not a player, store a reference
	//								openStorageInven = i.transform.parent.GetComponent<UiPlugger>().inven;
	//							}
	//						}
	//						ShiftClick(inventoryObject, openStorageInven, row, column, plug);
	//					}
	//				}
	//			}
				
	//		}
	//	}
	//	else if (slot != null) {
	//		//if the temp slot is not null, we know it is holding a valid inventory object. So, we must initiate the swap
	//		//if we are swapping two objects with the same name, prepare to stack!
	//		if(tempInven.array[tempRow, tempColumn].Name == inventoryObject.array[row, column].Name) {
	//			if(row == tempRow && tempColumn == column){
	//				//same name, same slot, same object, do nothing, reset
	//				ClearSlot();
	//			}
	//			else{
	//				//two different slots, but same name. merge stacks
	//				//check if you can just call them and keep it under that item's stack size
	//				if((tempInven.array[tempRow, tempColumn].Amount + inventoryObject.array[row, column].Amount) > inventoryObject.array[row, column].StackSize){
	//					//we cant do that, set the second buttons count to the max and subtract the necessary amount from the first button's amount
	//					tempInven.array[tempRow, tempColumn].Amount = ((inventoryObject.array[row, column].Amount + tempInven.array[tempRow, tempColumn].Amount) - inventoryObject.array[row, column].StackSize);
	//					//updateUI
	//					tempPlug.UpdateItem(tempRow, tempColumn, tempInven.array[tempRow, tempColumn].Amount);
	//					inventoryObject.array[row, column].Amount = inventoryObject.array[row, column].StackSize;
	//					//update UI
	//					plug.UpdateItem(row, column, inventoryObject.array[row, column].Amount);
	//					ClearSlot();
						
	//				}
	//				else{
	//					//Debug.Log("Stacking two stacks of same item type");
	//					//we can simply add the temp slot and second button press together
	//					//add the items in temp slot to the second pressed button's slot, clear out original button's slot and temp slot
	//					inventoryObject.array[row, column].Amount = tempInven.array[tempRow, tempColumn].Amount + inventoryObject.array[row, column].Amount;
	//					plug.UpdateItem(row, column, inventoryObject.array[row, column].Amount);
	//					tempInven.array[tempRow, tempColumn].Name = "";
	//					tempInven.array[tempRow, tempColumn].Amount = 0;
	//					tempInven.array[tempRow, tempColumn].image = emptyImage;
	//					tempPlug.ChangeItem(tempRow,tempColumn, emptyImage, 0, "");
	//					ClearSlot();
	//				}
	//			}
	//		}
	//		else{
	//			//clean swap, two different objects
	//			//we find the inventory slot the tempslot object is pointing to, and set it equal to the second button's data
	//			tempInven.array[tempRow, tempColumn] = inventoryObject.array[row, column];
	//			//we then update the Ui to follow suit
	//			tempPlug.ChangeItem(tempRow, tempColumn, inventoryObject.array[row, column].image, inventoryObject.array[row, column].Amount, inventoryObject.array[row, column].Name);
	//			//then we set the second button equal to the temp slot's data
	//			inventoryObject.array[row, column] = slot;
	//			//we also have the Ui update
	//			plug.ChangeItem(row,column, tempImage, tempCount, tempName);
	//			ClearSlot();		
	//		}
	//	}
	//}
	//The first half of swap, in its own method. This should allow me to do a click and drag instead of a click and click
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
			Debug.Log("The slot was not null in tempHolder, here data " + slot + ", " + slot.Name);
			//if the temp slot is not null, we know it is holding a valid inventory object. So, we must initiate the swap
			//if we are swapping two objects with the same name, prepare to stack!
			if(tempInven.array[tempRow, tempColumn].Name == inventoryObject.array[row, column].Name) {
				if(row == tempRow && tempColumn == column){
					Debug.Log("same object, doing nothing. heres some data " + inventoryObject.array[row, column].Name + ", " + row + ", " + column);
					//same name, same slot, same object, do nothing, reset
					ClearSlot();
				}
				else{
					Debug.Log("Different slots, same name, merging");
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
						Debug.Log("Stacking two stacks of same item type");
						//we can simply add the temp slot and second button press together
						//add the items in temp slot to the second pressed button's slot, clear out original button's slot and temp slot
						inventoryObject.array[row, column].Amount = tempInven.array[tempRow, tempColumn].Amount + inventoryObject.array[row, column].Amount;
						plug.UpdateItem(row, column, inventoryObject.array[row, column].Amount);
						tempInven.array[tempRow, tempColumn].Name = "";
						tempInven.array[tempRow, tempColumn].Amount = 0;
						tempInven.array[tempRow, tempColumn].image = emptyImage;
						tempPlug.ChangeItem(tempRow,tempColumn, emptyImage, 0, "");
						ClearSlot();
					}
				}
			}
			else{
				Debug.Log("Clean swap, two different objects, doing swap. Object 1 is "+ tempInven.array[tempRow, tempColumn].image.name + " and Object 2 is " + inventoryObject.array[row, column].image.name + " and finally, this is Slot: "+ slot.Name);
				//clean swap, two different objects
				//we find the inventory slot the tempslot object is pointing to, and set it equal to the second button's data
				tempInven.array[tempRow, tempColumn] = inventoryObject.array[row, column];
				//we then update the Ui to follow suit
				tempPlug.ChangeItem(tempRow, tempColumn, inventoryObject.array[row, column].image, inventoryObject.array[row, column].Amount, inventoryObject.array[row, column].Name);
				//then we set the second button equal to the temp slot's data
				inventoryObject.array[row, column] = slot;
				//we also have the Ui update
				plug.ChangeItem(row,column, tempImage, tempCount, tempName);
				ClearSlot();		
			}
		}
	}
}
