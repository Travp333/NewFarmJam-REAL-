using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimController : MonoBehaviour
{
	int isMovingHash;
	[SerializeField]
	Animator bodyAnim;
	Animator animator;
	PlayerStates state;
	[SerializeField]
	public GameObject player = default;
	// Start is called before the first frame update
    void Start()
	{
	    animator = GetComponent<Animator>();
		isMovingHash = Animator.StringToHash("isMoving");
		//isRollingHash = Animator.StringToHash("Rolling");
	    state = player.GetComponent<PlayerStates>();
    }

    // Update is called once per frame
    void Update()
	{
		bool isRolling = bodyAnim.GetBool("Rolling");
		bool isMoving = animator.GetBool(isMovingHash);
		bool movePressed = state.moving;
	    
		if(!isMoving && movePressed && player.GetComponent<Movement>().OnGround && !isRolling){
			animator.SetBool(isMovingHash, true);
		}
		if(isMoving && (!movePressed || !player.GetComponent<Movement>().OnGround || isRolling)){
			animator.SetBool(isMovingHash, false);
		}
    }
}
