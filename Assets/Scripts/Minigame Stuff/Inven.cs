using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random=UnityEngine.Random;

//This script allows inventory objects to be stored in a 2D array. This script should be able to be placed on both a player and a storage device. 
//This scirpt handles all the storage and manipulation of this array on the backend, ie picking up or dropping an object.
//Written by Conor and Travis

//This holds all the info we pull from a valid inventory object
public class ItemStat {

    public string Name = "";
    public int Amount = 0;
    public int StackSize = 0;
    public GameObject prefab = null;
	public Sprite image = null;
	public GameObject[] worldModel;
	public Item requiredIngredient;
    public Item craftsInto;
	public Item growsInto;
	public bool grabbable;
	public bool isSeed;
	public int age;
	public int matureAge;
	public int x, y;
	public Item harvestsInto;
	
}
public class Inven : MonoBehaviour
{
	[SerializeField]
	public Camera topDownCam;
	[SerializeField]
	[Tooltip("What object to spawn in the player's inventory (match with startingInvenCount, ie startingInvenPrefabs[0] will spawn startingInvenCount[0] times")]
	public Item[] startingInvenPrefabs;
	[SerializeField]
	public int[] startingInvenCount;
	[Tooltip("how many objects to spawn in the player's inventory (match with startingInvenCount, ie startingInvenPrefabs[0] will spawn startingInvenCount[0] times")]
	[SerializeField]
	Transform droppedItemSpawnPoint;
    [SerializeField]
    public GameObject UIPlugger;
    [SerializeField]
    public int hSize = 4;
    [SerializeField]
    public int vSize = 4;
    public bool isPickedUp = false;
    [HideInInspector]
    public Item item;
	public ItemStat [,] array;
	UiPlugger plug;
	public tempHolder temp;
	int loopCounter;
	int i2;
	int i3;
	string slot00Name, slot01Name, slot10Name, slot11Name;
	[SerializeField]
	bool debugLines;
	RecyclableItem recyclableItem;
	// Start is called before the first frame update
	protected void Update()
	{
		//if(debugLines){
			//	Debug.Log("slot00Name, " + slot00Name + "slot01Name, " + slot01Name + "slot10Name, "+ slot10Name + "slot10Name, " + slot11Name + "slot11Name");
			//}
	}
	public void Start()
	{
		temp = FindObjectOfType<tempHolder>();
		//stores reference to Ui object
		
		//This creates our 2D array based on the size given in editor
		array = new ItemStat[vSize,hSize];
		for (int i = 0; i < vSize; i++)
        {
			for (int i2 = 0; i2 < hSize; i2++)
            {
                array[i,i2] = new ItemStat();
	            array[i,i2].image = temp.emptyImage;
				array[i, i2].x = i;
				array[i, i2].y = i2;
            }
        }
		//Invoke("jumpStart", .1f);
	}
	public void EmptyInventorySlot(string coords){
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		ClearInfo(row, column);
		
		//updating UI to match new change
		plug.ClearSlot(row, column, temp.emptyImage);
	}
	public void DeductOneFromSlot(string coords){
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		array[row, column].Amount = array[row, column].Amount - 1;
		//you just dropped the last item in that slot, reverting to default
		if(array[row, column].Amount <= 0){
			//Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
			ClearInfo(row, column);
			
			//updating UI to match new change
			plug.ClearSlot(row, column, temp.emptyImage);
		}
		else{
			//there are still more of that item in the slot, updating UI to match new change
			plug.UpdateItem(row, column, array[row,column].Amount);
		}
		
	}
	public void jumpStart(){
		plug = UIPlugger.GetComponent<UiPlugger>();
		Invoke("lateStart", .01f);	
	}
	public void lateStart(){
		//Debug.Log("Late Start");
		foreach(Item g in startingInvenPrefabs){
			//Debug.Log(g);
			for (int i = 0; i < startingInvenCount[i2]; i++) {
				SmartPickUp(g);	
				if(isPickedUp){
					//Debug.Log("Successfull pickup!");
					isPickedUp = false;
				}
				else{
					//Debug.Log("No room in inventory, dropping on floor");
					//SpawnItem(g.GetComponent<pickUpableItem>().item.prefab);
				}
			}
			i2++;
		}
		i2 = 0;
	}
	public void SpecificPickUpAndCount(Item item, int row, int column, int amount){
		if(array[row,column].Name == ""){
			//yes empty, filling slot
			//Debug.Log("Slot (" + i + " , "+ i2 + " ) is empty, putting " + item.Objname + " in slot");
			isPickedUp = true;
			//Debug.Log("ispickedup set to "+ isPickedUp);
			array[row,column].Name = item.Objname;
			array[row,column].Amount = amount;
			array[row,column].StackSize = item.stackSize;
			array[row, column].image = item.img;
			array[row, column].worldModel = item.worldModel;
			array[row, column].requiredIngredient = item.requiredIngredient;
			array[row, column].craftsInto = item.craftsInto;
			array[row, column].growsInto = item.growsInto;
			array[row, column].grabbable = item.grabbable;
			array[row, column].isSeed = item.isSeed;
			array[row, column].age = item.age;
			array[row, column].matureAge = item.matureAge;
			array[row, column].harvestsInto = item.harvestsInto;
			//updating UI to match new change

			if (this.gameObject.tag != "Player"){
				plug.SyncWorldModel(row, column, array[row,column].Name, array[row, column].worldModel[Random.Range(0, item.worldModel.Length-1)]);
			}
			
			plug.ChangeItem(row, column, item.img, array[row,column].Amount, array[row,column].Name);
		}
		//no theres something here
		else{
			//Debug.Log("Slot (" + i + " , " + i2 + " ) has " + array[i,i2].Amount + " " + array[i,i2].Name + " in it, checking if it matches the new " + item.Objname);
			//basically is there room for it, is it the same object
			if(array[row,column].Name == item.Objname && array[row,column].StackSize !>= amount){
				//Debug.Log("Slot (" + i + " , "+ i2 + " ) has room, adding " + item.Objname + " to stack");
				//same object, room in the stack, adding to stack
				isPickedUp = true;
				//Debug.Log("ispickedup set to "+ isPickedUp);
				array[row,column].Amount = amount;
				//Debug.Log("we now have " + array[i,i2].Amount + " "+ array[i,i2].Name + " in " + "Slot (" + i + " , "+ i2 + " ) ");
				//updating UI to match new change*/
				plug.UpdateItem(row, column, array[row,column].Amount);
			}
			else if(array[row,column].StackSize <= array[row,column].Amount + 1){
				//Debug.Log("cant hold more than " + array[i,i2].Amount + " " + array[i,i2].Name + " in one stack, starting new stack... ");
			}
			//otherwise theres something here but its not the same type or theres no room for it
		}
	}
	public void SpecificPickUp(Item item, int row, int column){
		if(array[row,column].Name == ""){
			//yes empty, filling slot
			//Debug.Log("Slot (" + i + " , "+ i2 + " ) is empty, putting " + item.Objname + " in slot");
			isPickedUp = true;
			//Debug.Log("ispickedup set to "+ isPickedUp);
			array[row,column].Name = item.Objname;
			array[row,column].Amount = array[row,column].Amount + 1;
			array[row,column].StackSize = item.stackSize;
			array[row, column].image = item.img;
			array[row, column].worldModel = item.worldModel;
			array[row, column].requiredIngredient = item.requiredIngredient;
			array[row, column].craftsInto = item.craftsInto;
			array[row, column].growsInto = item.growsInto;
			array[row, column].grabbable = item.grabbable;
			array[row, column].isSeed = item.isSeed;
			array[row, column].age = item.age;
			array[row, column].matureAge = item.matureAge;
			array[row, column].harvestsInto = item.harvestsInto;
			

			//updating UI to match new change

			if (this.gameObject.tag != "Player"){
				plug.SyncWorldModel(row, column, array[row,column].Name, array[row, column].worldModel[Random.Range(0, item.worldModel.Length-1)]);
			}
			
			plug.ChangeItem(row, column, item.img, array[row,column].Amount, array[row,column].Name);
		}
		//no theres something here
		else{
			//Debug.Log("Slot (" + i + " , " + i2 + " ) has " + array[i,i2].Amount + " " + array[i,i2].Name + " in it, checking if it matches the new " + item.Objname);
			//basically is there room for it, is it the same object
			if(array[row,column].Name == item.Objname && array[row,column].StackSize !>= array[row,column].Amount + 1){
				//Debug.Log("Slot (" + i + " , "+ i2 + " ) has room, adding " + item.Objname + " to stack");
				//same object, room in the stack, adding to stack
				isPickedUp = true;
				//Debug.Log("ispickedup set to "+ isPickedUp);
				array[row,column].Amount = array[row,column].Amount + 1;
				//Debug.Log("we now have " + array[i,i2].Amount + " "+ array[i,i2].Name + " in " + "Slot (" + i + " , "+ i2 + " ) ");
				//updating UI to match new change
				plug.UpdateItem(row, column, array[row,column].Amount);
			}
			else if(array[row,column].StackSize <= array[row,column].Amount + 1){
				//Debug.Log("cant hold more than " + array[i,i2].Amount + " " + array[i,i2].Name + " in one stack, starting new stack... ");
			}
			//otherwise theres something here but its not the same type or theres no room for it
		}
	}
	//This handles picking up a new valid Inventory Item 
	public void PickUp(Item item){
		//Debug.Log("Made it to Pickup");
		//iterating through colomns
		for (int i = 0; i < vSize; i++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int i2 = 0; i2 < hSize; i2++)
			{
				//Debug.Log("Row" + i2);
				//is this slot empty?
				if(array[i,i2].Name == ""){
					//yes empty, filling slot
					//Debug.Log("Slot (" + i + " , "+ i2 + " ) is empty, putting " + item.Objname + " in slot");
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					array[i,i2].Name = item.Objname;
					array[i,i2].Amount = array[i,i2].Amount + 1;
					array[i,i2].StackSize = item.stackSize;
					array[i, i2].image = item.img;
					array[i, i2].worldModel = item.worldModel;
					array[i, i2].requiredIngredient = item.requiredIngredient;
					array[i, i2].craftsInto = item.craftsInto;
					array[i, i2].growsInto = item.growsInto;
					array[i, i2].grabbable = item.grabbable;
					array[i, i2].isSeed = item.isSeed;
					array[i, i2].age = item.age;
					array[i, i2].matureAge = item.matureAge;
					array[i, i2].harvestsInto = item.harvestsInto;
					
					//updating UI to match new change

					if (this.gameObject.tag != "Player"){
						plug.SyncWorldModel(i, i2, array[i,i2].Name, array[i, i2].worldModel[Random.Range(0, item.worldModel.Length-1)]);
					}
					
					plug.ChangeItem(i, i2, item.img, array[i,i2].Amount, array[i,i2].Name);
					i=0;
					i2=0;
					return;
				}
				//no theres something here
				else{
					//Debug.Log("Slot (" + i + " , " + i2 + " ) has " + array[i,i2].Amount + " " + array[i,i2].Name + " in it, checking if it matches the new " + item.Objname);
					//basically is there room for it, is it the same object
					if(array[i,i2].Name == item.Objname && array[i,i2].StackSize !>= array[i,i2].Amount + 1){
						//Debug.Log("Slot (" + i + " , "+ i2 + " ) has room, adding " + item.Objname + " to stack");
						//same object, room in the stack, adding to stack
						isPickedUp = true;
						//Debug.Log("ispickedup set to "+ isPickedUp);
						array[i,i2].Amount = array[i,i2].Amount + 1;
						//Debug.Log("we now have " + array[i,i2].Amount + " "+ array[i,i2].Name + " in " + "Slot (" + i + " , "+ i2 + " ) ");
						//updating UI to match new change
						plug.UpdateItem(i, i2, array[i,i2].Amount);
						i=0;
						i2=0;
						return;
					}
					else if(array[i,i2].StackSize <= array[i,i2].Amount + 1){
						//Debug.Log("cant hold more than " + array[i,i2].Amount + " " + array[i,i2].Name + " in one stack, starting new stack... ");
					}
					//otherwise theres something here but its not the same type or theres no room for it
				}
			}
		}
	}
	
	
	//this handles picking up new inventory items in a way that prioritizes existing stacks
	public void SmartPickUp(Item item){
		//Debug.Log("Made it to Smart Pickup");
		//Debug.Log("Starting "+ this.gameObject.name + " with a " + item.name);
		//iterating through colomns
		for (int i = 0; i < vSize; i++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int i2 = 0; i2 < hSize; i2++)
			{
				if((array[i,i2].Name == item.Objname) && (loopCounter <= (hSize * vSize)) && (array[i,i2].StackSize !>= array[i,i2].Amount + 1)){
					//found a stack of the existing item in inventory
					//Debug.Log("found a stack of the existing item in inventory");
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					array[i,i2].Amount = array[i,i2].Amount + 1;
					plug.UpdateItem(i,i2,array[i,i2].Amount);
					i=0;
					i2=0;
					loopCounter = 0;
					return;
				}
				else{
					//this slot doesnt have the same name or doesnt have space
					//Debug.Log("this slot doesnt have the same name or doesnt have space");
					//Debug.Log(loopCounter);
					loopCounter++;
					if(loopCounter >= (hSize * vSize)){
						//Debug.Log("searched whole inventory, nothing shares name, calling normal PickUp()");
						//searched whole inventory, nothing shares name, calling normal PickUp()
						PickUp(item);
						loopCounter = 0;
						return;
					}
				}
			}
		}
	}

	//This script actually Instantiates a new object with the same stats as the object stored in the inventory
	public void SpawnItem(GameObject item){
		GameObject b = Instantiate(item, droppedItemSpawnPoint.position, this.transform.rotation);
        b.GetComponent<Rigidbody>().velocity = this.gameObject.GetComponent<Rigidbody>().velocity * 2f;
	}
	//this drops a specific item that is found using its exact coordinates
	public void DropSpecificItem(string coords){
		
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		//Debug.Log( "Dropping "+row + ", " +column + " from inven " + this.gameObject.name);
		//parsing incoming string into ints
		//This slot does in fact have an object in it
		if(array[row, column].Amount > 0){
			//make the button flash grey for a second to give feedback
			plug.ButtonPress(row, column);
			Debug.Log("Dropping one " + array[row, column].Name + " from slot (" + row + " , "+ column + " ) , now we have" + (array[row,column].Amount - 1));
			//deduct one of the items from the stack
			array[row, column].Amount = array[row, column].Amount - 1;
			//spawn a prefab with the same info as that item
			//SpawnItem(array[row, column].prefab);
			//you just dropped the last item in that slot, reverting to default
            if(array[row, column].Amount <= 0){
	            //Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
				ClearInfo(row,column);
                //updating UI to match new change
	            plug.ClearSlot(row, column, temp.emptyImage);
            }
            else{
	            //there are still more of that item in the slot, updating UI to match new change
	            plug.UpdateItem(row, column, array[row,column].Amount);
            }
            return;
		}
		
	}
	public void DropWholeStack(string coords){
		//Debug.Log("Made it to inventory with coords "+ coords);
		
		//Debug.Log(temp.tempRow + ", " +temp.tempColumn);
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		//parsing incoming string into ints
		//This slot does in fact have an object in it
		if(array[row, column].Amount > 0){
			//make the button flash grey for a second to give feedback
			plug.ButtonPress(row, column);
			//spawn a prefab with the same info as that item
			//for (int i = 0; i < array[row, column].Amount; i++) {
			//	SpawnItem(array[row, column].prefab);
			//}
			array[row, column].Amount = 0;
			//you just dropped the last item in that slot, reverting to default
			if(array[row, column].Amount <= 0){
				//Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
				ClearInfo(row, column);
				//updating UI to match new change
				plug.ClearSlot(row, column, temp.emptyImage);
			}
			else{
				//there are still more of that item in the slot, updating UI to match new change
				plug.UpdateItem(row, column, array[row,column].Amount);
			}
			return;
		}
		
	}
	public void ClearInfo(int row, int column){
		array[row, column].Name = "";
		array[row, column].Amount = 0;
		array[row, column].StackSize = 0;
		array[row, column].prefab = null;
		array[row, column].image = temp.emptyImage;
		array[row, column].worldModel = null;
		array[row, column].requiredIngredient = null;
		array[row, column].craftsInto = null;
		array[row, column].growsInto = null;
		array[row, column].grabbable = false;
		array[row, column].isSeed = false;
		array[row, column].age = 0;
		array[row, column].matureAge = -1;
		array[row, column].harvestsInto = null;
	}
	public void PlantAgeUpdate() {
		plug = UIPlugger.GetComponent<UiPlugger>();
		foreach (ItemStat b in array)
		{
			if(b!= null){
				if (b.matureAge != -1)
				{
					b.age++;
					if (b.age >= b.matureAge && b.growsInto != null)
					{
						Item item = b.growsInto;
						b.Name = item.Objname;
						b.StackSize = item.stackSize;
						b.image = item.img;
						b.worldModel = item.worldModel;
						b.requiredIngredient = item.requiredIngredient;
						b.craftsInto = item.craftsInto;
						b.growsInto = item.growsInto;
						b.grabbable = item.grabbable;
						b.isSeed = item.isSeed;
						b.age = item.age;
						b.harvestsInto = item.harvestsInto;
						Debug.Log(b.Name + " " + b.age);
						b.matureAge = item.matureAge;
						if (b.worldModel != null)
						{
							plug.ClearWorldModel(b.x, b.y);
							plug.SyncWorldModel(b.x, b.y, b.Name, b.worldModel[Random.Range(0, b.worldModel.Length - 1)]);
						}
					}
					plug.ChangeItem(b.x, b.y, b.image, b.Amount, b.Name);
				}
			}
		}
	}
}
