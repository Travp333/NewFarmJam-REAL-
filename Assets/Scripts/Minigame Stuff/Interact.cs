using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//this script handles the interaction of the player character with objects in environment, like inventory objects
//Written by Conor and Travis

//BIG TODO
//close storage inventory if you get too far away from the object

public class Interact : MonoBehaviour
{
	Camera lastActiveCamera;
	//InputAction
	public InputAction interactAction;
	public InputAction openMenuAction;
	//reference to the tempHolder script
	tempHolder tempSlot;
	//Mask for the Raycast
    [SerializeField]
	LayerMask mask = default;
	[SerializeField]
	LayerMask intMask = default;
	//reference to the player UI
	[SerializeField]
	GameObject InventoryUI = null; //Inventory Canvas
	//raycast length;
	public float distance;
	//raycast hit holder
	RaycastHit hit;
	//reference to Inventory Script on Player
	Inven inv;
	// holder for inventory Items
	Item item;
	//bool to track inventory being open and closed
	bool invIsOpen = false;
	//List of every other inven script in the level
	[SerializeField]
	public List<GameObject> StorageInvenUI = new List<GameObject>();
	//tracks if a storage device's inventory is open
	public bool storageInvOpen = false;
	bool distanceGate = false;
	Transform storageObjectPos;
	[SerializeField]
	[Tooltip("The distance that the Inventory closes when walking away from an open storage object")]
	float invRange;
	[SerializeField]
	[Tooltip("How wide the sphere is that is cast from the player when hitting e")]
	float sphereCastRadius = .5f;
	PlayerStates states;
    void Start()
	{
		states = GetComponent<PlayerStates>();
		interactAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Interact");
		openMenuAction = GetComponent<PlayerInput>().currentActionMap.FindAction("OpenMenu");
		//plugging references
		tempSlot = this.gameObject.GetComponent<tempHolder>();
        inv = this.gameObject.GetComponent<Inven>();
		//disable all Inventory UI
		HideAllInventories();

	}
	public void HideAllInventories(){
		//Loop through all the UIPlugger objects in the scene and add them to a list while also disabling them.
		foreach(UiPlugger g in GameObject.FindObjectsOfType<UiPlugger>()){
			//avoids adding duplicates
			if(g.gameObject.transform.GetChild(0).gameObject.activeInHierarchy == true){
				if(StorageInvenUI.Contains(g.gameObject.transform.GetChild(0).gameObject)){
					//Debug.Log("Just hiding UI");
					g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
				}
				else{
					//Debug.Log("Hiding Ui and Adding to list");
					StorageInvenUI.Add(g.gameObject.transform.GetChild(0).gameObject);
					g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		}
	}
	public void HideAllNonPlayerInventories(){
		//Loop through all the UIPlugger objects in the scene and add them to a list while also disabling them.
		foreach(UiPlugger g in GameObject.FindObjectsOfType<UiPlugger>()){
			//avoids adding duplicates
			if(g.gameObject.tag != "Player"){
				if(g.gameObject.transform.GetChild(0).gameObject.activeInHierarchy == true){
					if(StorageInvenUI.Contains(g.gameObject.transform.GetChild(0).gameObject)){
						//Debug.Log("Just hiding UI");
						g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
					else{
						//Debug.Log("Hiding Ui and Adding to list");
						StorageInvenUI.Add(g.gameObject.transform.GetChild(0).gameObject);
						g.gameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
				}
			}
		}
	}
	//open up the player's Inventory
	void OpenInventory(){
		Debug.Log("SFX OPEN INVENTORY NOISE");
		this.gameObject.GetComponentInChildren<AnimationStateController>().ForceIdle();
		this.gameObject.GetComponent<Movement>().velocity = Vector3.zero;
		this.gameObject.GetComponent<Movement>().blockMovement();
		//makes sure the temp slot is empty
		tempSlot.ClearSlot();
		//puts cursor on screen
		Cursor.lockState = CursorLockMode.Confined; 
		//unhides cursor
		Cursor.visible = true;
		//enables UI object
		InventoryUI.SetActive(true);
		invIsOpen = true;
		
	}
	void DistanceCheck(){
		if(Vector3.Distance(this.transform.position, storageObjectPos.position) > invRange){
			Debug.Log("TOO FAR!!!!!");
			distanceGate = false;
			storageObjectPos = null;
			HideAllNonPlayerInventories();
			tempSlot.ClearSlot();
			storageInvOpen = false;
		}
	}
	
	//closes out the inventory and all open storage inventories, mostly just inverse of above
	void CloseInventory(){
		Debug.Log("SFX CLOSE INVENTORY NOISE");
		this.gameObject.GetComponent<Movement>().unblockMovement();
		tempSlot.ClearSlot();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		invIsOpen = false;
		HideAllInventories();
		storageInvOpen = false;
	}
	public void FlipCamera(Collider hit, bool flip){

		if(hit.transform.gameObject.tag != "Player"){
			if(hit.transform.gameObject.GetComponent<Inven>().topDownCam != null){
				if(flip){
					lastActiveCamera.enabled = true;
					hit.transform.gameObject.GetComponent<Inven>().topDownCam.enabled = false;
					GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera = lastActiveCamera;
				}
				else{
					GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera.enabled = false;
					hit.transform.gameObject.GetComponent<Inven>().topDownCam.enabled = true;
					lastActiveCamera = GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera;
					GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera = hit.transform.gameObject.GetComponent<Inven>().topDownCam;
					

				}
			}
		}
		
	}
    void Update()
	{
    	//inventory is not open
        if (openMenuAction.WasPressedThisFrame())
        {
        	if(invIsOpen){
        		CloseInventory();
        		HideAllInventories();
	        	Collider[] colliders = Physics.OverlapSphere(this.transform.position, sphereCastRadius);
	        	foreach (Collider hit in colliders)
	        	{
	        		if(hit.transform.gameObject.GetComponent<Inven>() != null){
	        			FlipCamera(hit, true);
	        		}
	        		
	        	}
        	}
        	else{
        		OpenInventory();
        	}
            
        }
        if (interactAction.WasPressedThisFrame())
        {
	        Collider[] colliders = Physics.OverlapSphere(this.transform.position, sphereCastRadius, intMask);
            foreach (Collider hit in colliders)
            {
	            if (invIsOpen)
	            {
		            CloseInventory();
		            FlipCamera(hit, true);
		            return;
	            }
            	//Debug.Log(hit.gameObject.name);
	            if(hit.transform.gameObject.GetComponent<Inven>() != null){
					if (hit.transform.gameObject.tag == "Plantable" || hit.transform.gameObject.tag == "Cooker" || hit.transform.gameObject.tag == "Chest")
					{
						FlipCamera(hit, false);
						//Debug.Log("HIT NON PLAYER INVENTORY");
						Inven inv = hit.transform.gameObject.GetComponent<Inven>();
						//enable the relevant UI element
						inv.UIPlugger.gameObject.transform.GetChild(0).gameObject.SetActive(true);
						storageInvOpen = true;
						//force open the player's inventory
						OpenInventory();
						//Add distance check here
						storageObjectPos = inv.gameObject.transform;
						distanceGate = true;
						return;
					}
					


				}
				else if (hit.transform.gameObject.tag == "Shop")
				{
					Shop shop = hit.transform.gameObject.GetComponent<Shop>();
					//enable the relevant UI element
					shop.shopMenu.SetActive(true);
					
					//force open the player's inventory
					OpenInventory();
					//Add distance check here
					storageObjectPos = inv.gameObject.transform;
					distanceGate = true;
					return;
				}

			}
        }
        
	    if(distanceGate){
	    	DistanceCheck();
	    }
    }
}
