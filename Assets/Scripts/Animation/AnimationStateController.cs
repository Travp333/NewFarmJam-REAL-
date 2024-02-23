using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I need to properly use hashes, im kinda half assing it here

public class AnimationStateController : MonoBehaviour
{
    public GameObject player = default;
	Movement sphere = default; 
	PlayerStates state;
	UpdateRotation rotation;
    Animator animator;
	int isRunningHash;
    int isOnSteepHash;
    int isJumpingHash;
    int onGroundHash;
	int isOnWallHash;
	int isWalkingHash;
	int isFallingHash;
    int isSwingingHash;
    bool isOnGround;
    bool isOnWall;
    bool isAttacking;
	bool moveBlockGate;
    [HideInInspector]
    public bool isOnGroundADJ;
    bool isOnSteep;
    [SerializeField]
    [Tooltip("how long you need to be in the air before the 'onGround' bool triggers")]
    float OnGroundBuffer = .5f;
    float Groundstopwatch = 0;
    
	public void BlockMovement(){
		sphere.blockMovement();
	}
	public void UnBlockMovement(){
		sphere.unblockMovement();
	}
    public void ResetSwinging(){
        animator.SetBool(isSwingingHash, false);
        animator.SetLayerWeight(1,0);
        isAttacking = false;
        state.macheteHitbox.SetActive(false);
    }

	void Start() { 
		state = player.GetComponent<PlayerStates>();
		rotation = player.transform.GetChild(0).GetComponent<UpdateRotation>();
        sphere = player.GetComponent<Movement>();
        animator = GetComponent<Animator>();
	    isRunningHash = Animator.StringToHash("isRunning");
	    isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
        onGroundHash = Animator.StringToHash("OnGround");
        isOnWallHash = Animator.StringToHash("isOnWall");
		isFallingHash = Animator.StringToHash("isFalling");
        isSwingingHash = Animator.StringToHash("isSwinging");
    }
    
	
    //this is meant to allow a sort of buffer, so bools stay true for a set amount of time
    void BoolAdjuster(){
        isOnGround = sphere.OnGround;
        isOnSteep = sphere.OnSteep;
        if (!isOnGround){
            Groundstopwatch += Time.deltaTime;
            if (Groundstopwatch >= OnGroundBuffer){
                isOnGroundADJ = false;
            }
        }
        if(isOnGround){
            Groundstopwatch = 0;
            isOnGroundADJ = true;
        }
    }
	
	void Update() {
	
        if(animator.GetBool("SwingResetter") == true){
            animator.SetBool("SwingResetter", false);
            ResetSwinging();
        }
		if(animator.GetBool("MoveBlocked") == true){
			if(!moveBlockGate){
				rotation.SnapRotationToDirection();
				sphere.blockMovement();
				moveBlockGate = true;	
			}
		}
		else{
			if(moveBlockGate){
				sphere.unblockMovement();
				moveBlockGate = false;
			}
		}
        //Debug.Log(sphere.velocity.magnitude);
        BoolAdjuster();
        isOnGround = isOnGroundADJ;
        bool isFalling = animator.GetBool(isFallingHash);
        bool isOnWall = animator.GetBool(isOnWallHash);
	    bool isRunning = animator.GetBool(isRunningHash);
	    bool isWalking = animator.GetBool(isWalkingHash);
		bool isJumping = animator.GetBool(isJumpingHash);
        isAttacking = animator.GetBool(isSwingingHash);
	    bool movementPressed = state.moving;
		bool WalkPressed = state.walking;
        bool attackPressed = state.attacking;

        if (isOnGroundADJ){
            animator.SetBool(onGroundHash, true);
        }
        else if (!isOnGroundADJ){
	        animator.SetBool(onGroundHash, false);
        }
        // if you are in the air, adding timer to give a little time before the falling animation plays
        if (!isOnGroundADJ && !isOnSteep){
            animator.SetBool(isFallingHash, true);
	        animator.SetBool(isRunningHash, false);
	        animator.SetBool(isWalkingHash, false);
        }
        else if (!isOnGroundADJ && isOnSteep){
            animator.SetBool(isOnWallHash, true);
        }
        if (isOnGroundADJ){
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isOnWallHash, false);
        }
        if (isOnSteep){
            animator.SetBool("isOnSteep", true);
        }
        if (!isOnSteep){
            animator.SetBool("isOnSteep", false);
            animator.SetBool(isOnWallHash, false);
        }
        if(!isAttacking && attackPressed && !sphere.moveBlocked){
            isAttacking = true;
            //Debug.Log("SWING!@!");
            animator.SetBool(isSwingingHash, true);
            animator.SetLayerWeight(1,1);
            state.macheteHitbox.SetActive(true);
        }
		if (!isWalking && (movementPressed && WalkPressed) && !sphere.moveBlocked){
		    animator.SetBool(isWalkingHash, true);
	    }
		if (isWalking && (!movementPressed || !WalkPressed)&& !sphere.moveBlocked){
		    animator.SetBool(isWalkingHash, false);
	    }
		if (!isRunning && movementPressed && !WalkPressed && sphere.velocity.magnitude > 0 && !sphere.moveBlocked ){
            animator.SetBool(isRunningHash, true);
	    }
		if (((isRunning && !movementPressed) || WalkPressed ||sphere.velocity.magnitude <= 0.08f) && !sphere.moveBlocked){
            animator.SetBool(isRunningHash, false);
	    }
    }

}
