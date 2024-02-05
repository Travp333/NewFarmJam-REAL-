using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStates : MonoBehaviour
{   
	[SerializeField]
	GameObject northPointer;
	[SerializeField]
	GameObject westPointer;
	[SerializeField]
	GameObject eastPointer;
	[SerializeField]
	GameObject southPointer;
	[SerializeField]
	GameObject root;
	[SerializeField]
	SimpleCameraMovement fpscamscript;
	Vector3 playerRotation;
	[SerializeField]
	UpdateRotation rot;
	[SerializeField]
	Camera ThirdPersonCam;
	Movement move;
	public bool walking;
	public bool moving;
	public InputAction movementAction;
	public InputAction walkAction;
	public InputAction interactAction;
	public InputAction attackAction;
	public InputAction turnLeftAction;
	public InputAction turnRightAction;
	[SerializeField]
    GameObject player = default;
    Movement sphere; 
	string facing = "North";
	Vector3 facingDir;
	[SerializeField]
	float rotSpeed;
	// Start is called before the first frame update
	void Awake()
	{
		transform.rotation = Quaternion.LookRotation( transform.forward , CustomGravity.GetUpAxis(transform.position));
        sphere = player.GetComponent<Movement>();
		move = this.GetComponent<Movement>();
		interactAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Interact");
		attackAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Attack");
		walkAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Walk");
		movementAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Move");
		turnLeftAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Turn Left");
		turnRightAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Turn Right");
	}

    // Update is called once per frame
    void Update()
	{

		if(walkAction.IsPressed() && !move.moveBlocked){
			walking = true;
		}
		else{
			walking = false;
		}
		if(turnLeftAction.WasPressedThisFrame() && !move.moveBlocked){
			if(facing == "North"){
				facing = "West";
				SnapRotationToWest();
			}
			else if(facing == "West"){
				facing = "South";
				SnapRotationToSouth();
			}
			else if(facing == "South"){
				facing = "East";
				SnapRotationToEast();
			}
			else if(facing == "East"){
				facing = "North";
				SnapRotationToNorth();
			}
			
		}
		if(turnRightAction.WasPressedThisFrame() && !move.moveBlocked){
			if(facing == "North"){
				facing = "East";
				SnapRotationToEast();
			}
			else if(facing == "West"){
				facing = "North";
				SnapRotationToNorth();
			}
			else if(facing == "South"){
				facing = "West";
				SnapRotationToWest();
			}
			else if(facing == "East"){
				facing = "South";
				SnapRotationToSouth();
			}
			
		}
        
	}
	void FixedUpdate()
	{
		if(this.transform.rotation.y != facingDir.y){
			Vector3 gravity = CustomGravity.GetUpAxis(this.transform.position);
			if(facing == "North"){
				Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(northPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, rotSpeed * Time.deltaTime);
			}
			else if(facing == "West"){
				Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(westPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, rotSpeed * Time.deltaTime);
			}
			else if(facing == "South"){
				Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(southPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, rotSpeed * Time.deltaTime);
			}
			else if(facing == "East"){
				Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(eastPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, rotSpeed * Time.deltaTime);
			}
			

		}
	}
	public void SnapRotationToNorth(){
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(northPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
		facingDir = player2Pointer;
	}
	public void SnapRotationToWest(){
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(westPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
		facingDir = player2Pointer; 
	}
	public void SnapRotationToSouth(){
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(southPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
		facingDir = player2Pointer;
	}
	public void SnapRotationToEast(){
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(eastPointer.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
		facingDir = player2Pointer;
	}
	Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}
