﻿
using UnityEngine;
using UnityEngine.InputSystem;
public class Movement : MonoBehaviour { 
	//CAN I DIFFERENTIATE BETWEEN CERTAIN TYPES OF STEEPS? ie a straight wall vs a sloped ramp? this would be nice!
	//This script controls the movement of the character. Adapted from https://catlikecoding.com/unity/tutorials/movement/ by Travis Parks
	PlayerStates state;
	[SerializeField]
	public GameObject center;

	[SerializeField]
	GameObject feet;
	public InputAction jumpAction;

	//reference to the script that controls limits on your movement speed
	public MovementSpeedController speedController;
	//the direction your jump goes in
	Vector3 jumpDirection;

	Vector3 lastContactNormal, lastSteepNormal;

	public GameObject parent;

	[SerializeField]
	[Tooltip("determines what rotation is relative to, ideally the camera")]
	public Transform playerInputSpace = default;

	float minGroundDotProduct, minStairsDotProduct;

	[SerializeField, Min(0f)]
	float probeDistance = 1f;

	[SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField, Range(0f, 90f)]
	float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range(0f, 100f)]
	[Tooltip("how quickly your character responds to input")]
	float maxAcceleration = 10f, maxAirAcceleration = 1f;

	[SerializeField, Range(0f, 100f)]
	[Tooltip("character's jump height")]
	float jumpHeight = 2f;

	[SerializeField, Range(0, 5)]
	[Tooltip("controls the amount of jumps you can do while in the air")]
	int maxAirJumps = 1;

	//all the masks that determine what interactions are valid with the player

	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1;
	
	[HideInInspector]
	public Rigidbody body, connectedBody; 
	Rigidbody previousConnectedBody;
	
	bool desiredJump;

	[HideInInspector]
	public int groundContactCount, steepContactCount;

	[HideInInspector]

	public bool OnGround {
		get {
			return groundContactCount > 0;
		}
	}

	public bool OnSteep {
		get {
			return steepContactCount > 0;
		}
	}
	int jumpPhase;
	int stepsSinceLastGrounded, stepsSinceLastJump;

	Vector3 contactNormal, steepNormal;

	Vector3 upAxis, rightAxis;
	[HideInInspector]
	public Vector3 forwardAxis;

	Vector3 connectionWorldPosition, connectionLocalPosition;
	[HideInInspector]
	public Vector3 playerInput;
	[HideInInspector]
	public Vector3 velocity; 
	Vector3 connectionVelocity;

	bool skip = true;

	public bool moveBlocked;
	//private void OnEnable()
	//{
	//	OptionsMenuManager.Unstuck += ResetYPos;
	//}
   // private void OnDisable()
   // {
	//	OptionsMenuManager.Unstuck -= ResetYPos;

	//}
	void ResetYPos()
    {
		transform.position = new Vector3(transform.position.x, 4, transform.position.z);
    }
	public void blockMovement(){
		//Debug.Log("Blocked Movement in movement.cs!");
		moveBlocked = true;
		playerInput.x = 0f;
		playerInput.y = 0f;
		velocity = Vector3.zero;
	}
	public void unblockMovement(){
		moveBlocked = false;

	}
	//runs when object becomes active
	void Awake () {
		state = GetComponent<PlayerStates>();
		jumpAction = GetComponent<PlayerInput>().currentActionMap.FindAction("Jump");
		speedController = GetComponent<MovementSpeedController>();
		body = GetComponent<Rigidbody>();
		//turn gravity off for the rigid body
		body.useGravity = false;
		//call validate ?
		OnValidate();
	}
	//runs every frame
	void Update () {
		//responds to the jump keybind to allow jumping
		//Debug.Log("IS ON GROUND "+ OnGround);
		//Debug.Log("IS ON STEEP "+ OnSteep);
		desiredJump |= jumpAction.WasPressedThisFrame() && !moveBlocked;
		//stores the horizontal and vertical input axes
		if(!moveBlocked){
			playerInput.x = state.movementAction.ReadValue<Vector2>().x;
			playerInput.y = state.movementAction.ReadValue<Vector2>().y;
			playerInput = Vector3.ClampMagnitude(playerInput, 1f);
		}
		//redirects the characters input to be relative to a "playerinputspace" object, if it is given. usually, this will be the camera
		if (playerInputSpace) {
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis =
				ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
		}
		//if there is no playerinputspace object it will just be relative to the world
		else	{
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}
	}
	
	void FixedUpdate() {
		Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
		UpdateState();
		AdjustVelocity();
		if (desiredJump) {
			desiredJump = false;
			Jump(gravity);
		}
		else if (OnGround && velocity.sqrMagnitude < 0.01f) {
			velocity +=
				contactNormal *
				(Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
		}
		else {
			velocity += gravity * Time.deltaTime;
		}
		if(body.isKinematic == false){
			body.velocity = velocity;
		}
		ClearState();
	}

	bool CheckSteepContacts () {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
			float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot >= minGroundDotProduct) {
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	float GetMinDot (int layer) {
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}

	bool SnapToGround () {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed) {
			return false;
		}
		if (!Physics.Raycast(
			// i changed the first argument to be  the "feet" empty tied to the player character. this may be causing jitters (parent.transform.GetChild(1))
			//parent.transform.GetChild(3) on FPS
			feet.transform.position, -upAxis, out RaycastHit hit,
			probeDistance, probeMask, QueryTriggerInteraction.Ignore
			)) {
			return false;
		}
		float upDot = Vector3.Dot(upAxis, hit.normal);
		if (upDot < GetMinDot(hit.collider.gameObject.layer)) {
			return false;
		}
		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f) {
		velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		connectedBody = hit.rigidbody;
		return true;
	}

	void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
	}

	void ClearState (){
		lastContactNormal = contactNormal;
		lastSteepNormal = steepNormal;
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = connectionVelocity = Vector3.zero;
		previousConnectedBody = connectedBody;
		connectedBody = null;
		lastContactNormal = contactNormal;
	}

	public void PreventSnapToGround () {
		stepsSinceLastJump = -1;
	}

	void UpdateState(){
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;
		if ( OnGround || SnapToGround() || CheckSteepContacts()){
			stepsSinceLastGrounded = 0;
			if (stepsSinceLastJump > 1) {
				jumpPhase = 0;
			}
			if (groundContactCount > 1){
				contactNormal.Normalize();
			}
			contactNormal.Normalize();
		}
		else {
			contactNormal = upAxis;
		}
		if (connectedBody) {
			if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
				UpdateConnectionState();
			}
		}
	}

	void UpdateConnectionState () {
		if (connectedBody == previousConnectedBody) {
			Vector3 connectionMovement =
				connectedBody.transform.TransformPoint(connectionLocalPosition) - 
				connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
			connectionWorldPosition = body.position;
			connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
				connectionWorldPosition
			);
		}
	}

	public void JumpTrigger(){
		desiredJump = true;
	}
	
	void Jump(Vector3 gravity) {
		if (OnGround) {
				jumpDirection = contactNormal;
			}
		//else if (OnSteep && !state.crouching && !state.holding) {
		//		jumpDirection = steepNormal;
				// this was originally 0 but i changed it so that wall jumping doesnt count as one of your air jumps
		//		jumpPhase -= 1;
		//	}
		else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
			if (jumpPhase == 0) {
			jumpPhase = 1;
			}
			jumpDirection = contactNormal;
		}
		else {
			return;
		}

			if (skip){
				stepsSinceLastJump = 0;
				jumpPhase += 1;
				float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
				jumpDirection = (jumpDirection + upAxis).normalized;
				float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
				if (alignedSpeed > 0f) {
					jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
				}
				velocity += jumpDirection * jumpSpeed;
			}
			else{
				skip = true;
			}
		}

	void OnCollisionEnter (Collision collision) {
		EvaluateCollision(collision);
	}

	void OnCollisionStay (Collision collision) {
		EvaluateCollision(collision);
	}


	void EvaluateCollision (Collision collision) {
		int layer = collision.gameObject.layer;
		float minDot = GetMinDot(layer);
		for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
			float upDot = Vector3.Dot(upAxis, normal);
			//This was just > than for conor and i, but on the tutorial it was changed to >= without any explanation so keep that in mind
			if (upDot >= minDot) {
				connectedBody = collision.rigidbody;
				groundContactCount += 1;
				contactNormal += normal;
			}
			else {
				if (upDot > -0.01f) {
					steepContactCount += 1;
					steepNormal += normal;
					if (groundContactCount == 0) {
						connectedBody = collision.rigidbody;
					}
				}

			}
		}
	}
// these two statements are equal (question mark notation reminder)


//	movement *= speed * ( ( absJoyPos.x > absJoyPos.y ) ? absJoyPos.x : absJoyPos.y );

//	movement *= speed;
//	If( absJoyPos.x > absJoyPos.y )
//	{
//	movement *= absJoyPos.x;
//	}
//	else
//	{
//	movement *= absJoyPos.y;
//	}

// basically a = b ? c:d; means a is either c or d depending on b, or 
// if(b){
// a = c
// }
// else {
// a = d
// }

	void AdjustVelocity () {
		float acceleration, speed;
		Vector3 xAxis, zAxis;
		acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		speed = speedController.currentSpeed;
		xAxis = rightAxis;
		zAxis = forwardAxis;
		
		
		xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
		zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

		Vector3 relativeVelocity = velocity - connectionVelocity;

		float currentX = Vector3.Dot(relativeVelocity, xAxis);
		float currentZ = Vector3.Dot(relativeVelocity, zAxis);

		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX =
			Mathf.MoveTowards(currentX, playerInput.x * speed, maxSpeedChange);
		float newZ =
			Mathf.MoveTowards(currentZ, playerInput.y * speed, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}
	public Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}
