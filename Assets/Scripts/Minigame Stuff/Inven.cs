using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
public class Inven : MonoBehaviour
{
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
	tempHolder temp;
	int loopCounter;
	int i2;
	int i3;
	RecyclableItem recyclableItem;
    // Start is called before the first frame update
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
            }
        }
		//Invoke("jumpStart", .1f);
	}
	public void EmptyInventorySlot(string coords){
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		array[row, column].Name = "";
		array[row, column].Amount = 0;
		array[row, column].StackSize = 0;
		array[row, column].prefab = null;
		array[row, column].image = temp.emptyImage;
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
			Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
			array[row, column].Name = "";
			array[row, column].Amount = 0;
			array[row, column].StackSize = 0;
			array[row, column].prefab = null;
			array[row, column].image = temp.emptyImage;
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
		foreach( Item g in startingInvenPrefabs){
			for (int i = 0; i < startingInvenCount[i2]; i++) {
				SmartPickUp(g);	
				if(isPickedUp){
					Debug.Log("Successfull pickup!");
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

	//This handles picking up a new valid Inventory Item 
	public void PickUp(Item item){
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
					Debug.Log("Slot (" + i + " , "+ i2 + " ) is empty, putting " + item.Objname + " in slot");
					isPickedUp = true;
					//Debug.Log("ispickedup set to "+ isPickedUp);
					array[i,i2].Name = item.Objname;
					array[i,i2].Amount = array[i,i2].Amount + 1;
					array[i,i2].StackSize = item.stackSize;
					array[i, i2].image = item.img;
					//updating UI to match new change
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
						Debug.Log("cant hold more than " + array[i,i2].Amount + " " + array[i,i2].Name + " in one stack, starting new stack... ");
					}
					//otherwise theres something here but its not the same type or theres no room for it
				}
			}
		}
	}
	
	//Overload that allows picking up itemstat objects
	public void PickUp(ItemStat item){
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
					array[i,i2].Name = item.Name;
					array[i,i2].Amount = array[i,i2].Amount + 1;
					array[i,i2].StackSize = item.StackSize;
					array[i,i2].prefab = item.prefab;
					array[i, i2].image = item.image;
					//updating UI to match new change
					plug.ChangeItem(i, i2, item.image, array[i,i2].Amount, array[i,i2].Name);
					i=0;
					i2=0;
					return;
				}
				//no theres something here
				else{
					//Debug.Log("Slot (" + i + " , " + i2 + " ) has " + array[i,i2].Amount + " " + array[i,i2].Name + " in it, checking if it matches the new " + item.Objname);
					//basically is there room for it, is it the same object
					if(array[i,i2].Name == item.Name && array[i,i2].StackSize !>= array[i,i2].Amount + 1){
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
		Debug.Log("Starting "+ this.gameObject.name + " with a " + item.name);
		//iterating through colomns
		for (int i = 0; i < vSize; i++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int i2 = 0; i2 < hSize; i2++)
			{
				if((array[i,i2].Name == item.Objname) && (loopCounter <= (hSize * vSize)) && (array[i,i2].StackSize !>= array[i,i2].Amount + 1)){
					//found a stack of the existing item in inventory
					Debug.Log("found a stack of the existing item in inventory");
					isPickedUp = true;
					Debug.Log("ispickedup set to "+ isPickedUp);
					array[i,i2].Amount = array[i,i2].Amount + 1;
					plug.UpdateItem(i,i2,array[i,i2].Amount);
					i=0;
					i2=0;
					loopCounter = 0;
					return;
				}
				else{
					//this slot doesnt have the same name or doesnt have space
					Debug.Log("this slot doesnt have the same name or doesnt have space");
					//Debug.Log(loopCounter);
					loopCounter++;
					if(loopCounter >= (hSize * vSize)){
						Debug.Log("searched whole inventory, nothing shares name, calling normal PickUp()");
						//searched whole inventory, nothing shares name, calling normal PickUp()
						PickUp(item);
						loopCounter = 0;
						return;
					}
				}
			}
		}
	}
	//overload that allows picking up itemstat objects
	public void SmartPickUp(ItemStat item){
		//Debug.Log("Starting "+ this.gameObject.name + " with a " + item.name);
		//iterating through colomns
		for (int i = 0; i < vSize; i++)
		{
			//Debug.Log("Column " + i);
			//iterating through rows
			for (int i2 = 0; i2 < hSize; i2++)
			{
				if((array[i,i2].Name == item.Name) && (loopCounter <= (hSize * vSize)) && (array[i,i2].StackSize !>= array[i,i2].Amount + 1)){
					//found a stack of the existing item in inventory
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
					//Debug.Log(loopCounter);
					loopCounter++;
					if(loopCounter >= (hSize * vSize)){
						//Debug.Log("made it to finishLine");
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
		//Debug.Log(temp.tempRow + ", " +temp.tempColumn);
		string [] coords2 = coords.Split(",");
		int row = int.Parse(coords2[0]);
		int column = int.Parse(coords2[1]);
		if(!((temp.tempRow == row)&&(temp.tempColumn == column))){
			//parsing incoming string into ints
			//This slot does in fact have an object in it
			if(array[row, column].Amount > 0){
				//make the button flash grey for a second to give feedback
				plug.ButtonPress(row, column);
				//Debug.Log("Dropping one " + array[row, column].Name + " from slot (" + row + " , "+ column + " ) , now we have" + (array[row,column].Amount - 1));
				//deduct one of the items from the stack
				array[row, column].Amount = array[row, column].Amount - 1;
				//spawn a prefab with the same info as that item
				SpawnItem(array[row, column].prefab);
				//you just dropped the last item in that slot, reverting to default
	            if(array[row, column].Amount <= 0){
		            //Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
	                array[row, column].Name = "";
	                array[row, column].Amount = 0;
	                array[row, column].StackSize = 0;
	                array[row, column].prefab = null;
		            array[row, column].image = temp.emptyImage;
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
			for (int i = 0; i < array[row, column].Amount; i++) {
				SpawnItem(array[row, column].prefab);
			}
			array[row, column].Amount = 0;
			//you just dropped the last item in that slot, reverting to default
			if(array[row, column].Amount <= 0){
				//Debug.Log("Out of " + array[row, column].Name + " in slot (" + row + " , "+ column + " ) , slot now empty ");
				array[row, column].Name = "";
				array[row, column].Amount = 0;
				array[row, column].StackSize = 0;
				array[row, column].prefab = null;
				array[row, column].image = temp.emptyImage;
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

}
