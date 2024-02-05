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
    bool isOnGround;
    bool isOnWall;
	bool moveBlockGate;
    [HideInInspector]
    public bool isOnGroundADJ;
    bool isOnSteep;
    //bool isOnSteepADJ;
    bool JumpPressed;
    [SerializeField]
    [Tooltip("how long you need to be in the air before the 'onGround' bool triggers")]
    float OnGroundBuffer = .5f;
    [SerializeField]
    [Tooltip("how long isJumping stays true after pressing it ( maybe should be in movingsphere?)")]
    float JumpBuffer = .5f;
    bool JumpSwitch = true;
    float Groundstopwatch = 0;
	float Jumpstopwatch = 0;
    


	public void JumpAnimEvent(){
		sphere.JumpTrigger();
    }
	public void BlockMovement(){
		sphere.blockMovement();
	}
	public void UnBlockMovement(){
		sphere.unblockMovement();
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
    }
    
	
    //this is meant to allow a sort of buffer, so bools stay true for a set amount of time
    void BoolAdjuster(){
        isOnGround = sphere.OnGround;
        isOnSteep = sphere.OnSteep;
        if (!isOnGround && !JumpPressed){
            Groundstopwatch += Time.deltaTime;
            if (Groundstopwatch >= OnGroundBuffer){
                isOnGroundADJ = false;
            }
        }
        if (!isOnGround && JumpPressed){
            isOnGroundADJ = false;
        }
        if(isOnGround){
            Groundstopwatch = 0;
            isOnGroundADJ = true;
        }
    }
	public void ResetArmedLayerWeight(){
		animator.SetLayerWeight(1, 0);
	}
	public void EnterNullState(){
		animator.Play("NullState", 1);
	}
	public void ExitNullState(){
		animator.Play("Sling empty", 1);
	}
	
    float jumpCount;
    float jumpCap = .2f;
	void Update() {
	
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
	    bool JumpPressed = sphere.jumpAction.IsPressed();
        isOnGround = isOnGroundADJ;
        bool isFalling = animator.GetBool(isFallingHash);
        bool isOnWall = animator.GetBool(isOnWallHash);
	    bool isRunning = animator.GetBool(isRunningHash);
	    bool isWalking = animator.GetBool(isWalkingHash);
		bool isJumping = animator.GetBool(isJumpingHash);
	    bool movementPressed = state.moving;
		bool WalkPressed = state.walking;

        if (isOnGroundADJ){
            animator.SetBool(onGroundHash, true);
        }
        else if (!isOnGroundADJ){
	        animator.SetBool(onGroundHash, false);
        }
        //This makes jump stay true a little longer after you press it, dependent on "JumpBuffer"
		if (JumpPressed && !sphere.moveBlocked){
            if(JumpSwitch){
                Jumpstopwatch = 0;
                animator.SetBool(isJumpingHash, true);
                JumpSwitch = false;
            }
            else{
                Jumpstopwatch += Time.deltaTime;
                    if(Jumpstopwatch >= JumpBuffer){
                        animator.SetBool(isJumpingHash, false);
                    }
            }   
        }
        //this activates when jump is not pressed, counts until jumpbuffer, then disables jump
        if(!JumpPressed){
            JumpSwitch = true;
            Jumpstopwatch += Time.deltaTime;
            if(Jumpstopwatch >= JumpBuffer){
                animator.SetBool(isJumpingHash, false);
            }
        }
        // if you are in the air, adding timer to give a little time before the falling animation plays
        if (!isOnGroundADJ && !isOnSteep){
            jumpCount += Time.deltaTime;
            if(jumpCount > jumpCap){
                animator.SetBool(isFallingHash, true);
	            animator.SetBool(isRunningHash, false);
	            animator.SetBool(isWalkingHash, false);
                jumpCount = 0f;
            }
        }
        else if(isOnGroundADJ || isOnSteep){
            jumpCount = 0f;
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
