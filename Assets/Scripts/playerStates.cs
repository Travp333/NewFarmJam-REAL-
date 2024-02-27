using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStates : MonoBehaviour
{   
	[SerializeField]
	public GameObject macheteHitbox;
	[SerializeField]
	GameObject root;
	Vector3 playerRotation;
	[SerializeField]
	UpdateRotation rot;
	[SerializeField]
	public Camera ThirdPersonCam;
	Movement move;
	public bool walking;
	public bool moving;
	public bool attacking;
	public InputAction openMenuAction;
	public InputAction movementAction;
	public InputAction walkAction;
	public InputAction interactAction;
	public InputAction attackAction;
	[SerializeField]
    GameObject player = default;
    Movement sphere; 
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
		if(attackAction.IsPressed() && !move.moveBlocked){
			//Debug.Log("SWIONG!!");
			attacking = true;
		}
		else{
			attacking = false;
		}
        
	}

	Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}
