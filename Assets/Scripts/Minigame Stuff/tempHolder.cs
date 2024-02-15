using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
//This script handles the Swap mechanic in the inventory. it has a tempslot that we store inventory objects we are looking to swap in,
//and also handles the logic of doing the actual swap.
//written by Conor and Travis


//Currently broken

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
	public Item tempCraftsInto;
	public Item tempGrowsInto;
	public bool tempGrabbable;
	public Item tempRequiredIngredient;
	
	//protected void Update()
	//{
		//if(tempRequiredIngredient != null){
			//	Debug.Log("TRI: " + tempRequiredIngredient);
			//}
		//}
	
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
		tempModel = null;
		tempCraftsInto = null;
		tempGrowsInto = null;
		tempRequiredIngredient = null;
		tempGrabbable = false;
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
				tempModel = slot.worldModel[Random.Range(0,slot.worldModel.Length-1)];
				tempCraftsInto = slot.craftsInto;
				tempGrowsInto = slot.growsInto;
				tempRequiredIngredient = slot.requiredIngredient;
				tempGrabbable = slot.grabbable;
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
		openStorageInven = plug.inven;
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
						tempInven.array[tempRow, tempColumn].image = emptyImage;; 
						tempPlug.ChangeItem(tempRow,tempColumn, emptyImage, 0, "");
						tempModel = null;
						tempCraftsInto = null;
						tempGrowsInto = null;
						tempRequiredIngredient = null;
						tempGrabbable = false;
						ClearSlot();
					}
				}
			}
			
			else{
				if(tempCraftsInto != null && tempRequiredIngredient == null && inventoryObject.gameObject.tag == "Plantable" && inventoryObject.array[row, column].Name == ""){
					GameObject tempModel2 = tempCraftsInto.worldModel[Random.Range(0,tempCraftsInto.worldModel.Length-1)];
					Debug.Log("Crafting with nothing!");
					inventoryObject.array[row, column].Name = tempCraftsInto.name;
					inventoryObject.array[row, column].Amount = 1;
					inventoryObject.array[row, column].StackSize = tempCraftsInto.stackSize;
					inventoryObject.array[row, column].image = tempCraftsInto.img;
					inventoryObject.array[row, column].worldModel = tempCraftsInto.worldModel;
					inventoryObject.array[row, column].growsInto = tempCraftsInto.growsInto;
					inventoryObject.array[row, column].requiredIngredient = tempCraftsInto.requiredIngredient;
					inventoryObject.array[row, column].craftsInto = tempCraftsInto.craftsInto;
					inventoryObject.array[row, column].grabbable = tempCraftsInto.grabbable;
					plug.SyncWorldModel(row, column, tempCraftsInto.name, tempModel2);
					plug.ChangeItem(row,column, tempCraftsInto.img, 1 , tempCraftsInto.name);
					tempInven.DropSpecificItem(tempRow.ToString() +", "+ tempColumn.ToString());
					if(tempInven.array[tempRow, tempColumn].Amount <= 0){
						tempPlug.ClearWorldModel(tempRow, tempColumn);
						tempPlug.ClearSlot(tempRow, tempColumn, emptyImage);
					}
					else{
						tempPlug.UpdateItem(tempRow, tempColumn, tempInven.array[tempRow, tempColumn].Amount);
					}

					ClearSlot();
					//if(tempRequiredIngredient.name == inventoryObject.array[row, column].Name){
						//PUT STUFF HERE
					//}
				}
				else if(inventoryObject.array[row, column].image.name == "empty"){
					Debug.Log("Clean swap, two different objects, doing swap. Object 1 is "+ tempInven.array[tempRow, tempColumn].Name + " with " + tempInven.array[tempRow, tempColumn].Amount + " remaining in stock, and Object 2 is " + inventoryObject.array[row, column].Name + " with " + inventoryObject.array[row, column].Amount + "remaining in stock, and finally, this is Slot: "+ row + ", " + column);
					//clean swap, two different objects
					//STILL A PROBLEM HERE WHEN SWAPPING TWO ITEMS, WORLD MODELS STACK ON EACHOTHER AND GET A NULL ERROR EVENTUALLY 
					//we find the inventory slot the tempslot object is pointing to, and set it equal to the second button's data
					tempInven.array[tempRow, tempColumn] = inventoryObject.array[row, column];
					tempCraftsInto = inventoryObject.array[row, column].craftsInto;
					tempGrowsInto = inventoryObject.array[row, column].growsInto;
					tempRequiredIngredient = inventoryObject.array[row, column].requiredIngredient;
					tempGrabbable = inventoryObject.array[row, column].grabbable;
					//we then update the Ui to follow suit
					tempPlug.ClearWorldModel(tempRow, tempColumn);
					tempPlug.ChangeItem(tempRow, tempColumn, inventoryObject.array[row, column].image, inventoryObject.array[row, column].Amount, inventoryObject.array[row, column].Name);
					//then we set the second button equal to the temp slot's data
					inventoryObject.array[row, column] = slot;
					inventoryObject.array[row, column].growsInto = slot.growsInto;
					inventoryObject.array[row, column].requiredIngredient = slot.requiredIngredient;
					inventoryObject.array[row, column].craftsInto = slot.craftsInto;
					inventoryObject.array[row, column].grabbable = slot.grabbable;
					//we also have the Ui update
					plug.SyncWorldModel(row, column, tempName, tempModel);
					plug.ChangeItem(row,column, tempImage, inventoryObject.array[row, column].Amount, tempName);
					ClearSlot();		
				}
				//Debug.Log(tempInven.array[tempRow, tempColumn].requiredIngredient.name +", "0 + tempInven.array[tempRow, tempColumn].Name + ", "+ inventoryObject.array[tempRow, tempColumn].requiredIngredient.name + ", " + )
				else if((tempRequiredIngredient != null && inventoryObject != null)){
					Debug.Log("Dropped on real object! " + tempRequiredIngredient.name + " and " +inventoryObject.array[row, column].Name);
					if(tempRequiredIngredient.name == inventoryObject.array[row, column].Name){
						//Debug.Log("Matching requirement and name");
						if(tempCraftsInto != null){
							if((inventoryObject.array[row, column].requiredIngredient.name == tempInven.array[tempRow, tempColumn].Name || tempInven.array[tempRow, tempColumn].requiredIngredient.name == inventoryObject.array[row, column].Name)&&inventoryObject.gameObject.tag == "Plantable"){
								Debug.Log("Crafting with something! making " + tempCraftsInto.name);
								inventoryObject.array[row, column].Name = tempCraftsInto.name;
								inventoryObject.array[row, column].Amount = 1;
								inventoryObject.array[row, column].StackSize = tempCraftsInto.stackSize;
								inventoryObject.array[row, column].image = tempCraftsInto.img;
								inventoryObject.array[row, column].worldModel = tempCraftsInto.worldModel;
								inventoryObject.array[row, column].growsInto = tempCraftsInto.growsInto;
								inventoryObject.array[row, column].requiredIngredient = tempCraftsInto.requiredIngredient;
								inventoryObject.array[row, column].craftsInto = tempCraftsInto.craftsInto;
								inventoryObject.array[row, column].grabbable = tempCraftsInto.grabbable;
								plug.ClearWorldModel(row, column);
								plug.SyncWorldModel(row, column, tempCraftsInto.name, tempCraftsInto.worldModel[Random.Range(0,tempCraftsInto.worldModel.Length-1)]);
								plug.ChangeItem(row,column, tempCraftsInto.img, 1 , tempCraftsInto.name);
								tempInven.DropSpecificItem(tempRow.ToString() +", "+ tempColumn.ToString());
								if(tempInven.array[tempRow, tempColumn].Amount <= 0){
									tempPlug.ClearWorldModel(tempRow, tempColumn);
									tempPlug.ClearSlot(tempRow, tempColumn, emptyImage);
								}
								else{
									tempPlug.UpdateItem(tempRow, tempColumn, tempInven.array[tempRow, tempColumn].Amount);
								}
	
								ClearSlot();
								//if(tempRequiredIngredient.name == inventoryObject.array[row, column].Name){
								//PUT STUFF HERE
								//}
							}
							//Debug.Log("creating new object");	
							//Debug.Log("TempRow+Column" + tempRow + ", " + tempColumn);
							//Debug.Log("Row+Column" + row + ", " + column);
							//tempInven.DropSpecificItem(tempRow.ToString() +", "+ tempColumn.ToString());
							//inventoryObject.DropSpecificItem(row.ToString() + ", " + column.ToString());
							//ClearSlot();
						}
						else{
							//Debug.Log("Clearing slot via crafting");	
							//Debug.Log("TempRow+Column" + tempRow + ", " + tempColumn);
							//Debug.Log("Row+Column" + row + ", " + column);
							tempInven.DropSpecificItem(tempRow.ToString() +", "+ tempColumn.ToString());
							inventoryObject.DropSpecificItem(row.ToString() + ", " + column.ToString());
							plug.ClearWorldModel(row, column);
							if(inventoryObject.array[tempRow, tempColumn].Amount <=0){
								tempPlug.ClearWorldModel(tempRow,tempColumn);
							}
							//Debug.Log("TOTAL COUNT SLOT 2: "+ inventoryObject.array[row, column].Name + ", " + inventoryObject.array[row, column].Amount );
							//Debug.Log("TOTAL COUNT SLOT 1: "+ tempInven.array[tempRow, tempColumn].Name + ", " + inventoryObject.array[tempRow, tempColumn].Amount );
							ClearSlot();
						}
					
						
					}
					else{
						ClearSlot();
					}
				}
				else{
					ClearSlot();
				}
			}
			
		}
	}
}
