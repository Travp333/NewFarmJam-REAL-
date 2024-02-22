using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStates : MonoBehaviour
{   [SerializeField]
	bool turnSnapping;
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
	Vector3 playerRotation;
	[SerializeField]
	UpdateRotation rot;
	[SerializeField]
	Camera ThirdPersonCam;
	Movement move;
	public bool walking;
	public bool moving;
	public InputAction openMenuAction;
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
		transform.rotation = Quaternion.LookRotation( transform.forward, CustomGravity.GetUpAxis(transform.position));
        sphere = player.GetComponent<Movement>();
		move = this.GetComponent<Movement>();
		interactAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Interact");
		openMenuAction = GetComponent<PlayerInput>().currentActionMap.FindAction("OpenMenu");
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
        
	}

	Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}
