using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//updates what direction the player model is facing
public class UpdateRotation : MonoBehaviour

{
	[SerializeField]
	GameObject point;
	[SerializeField]
	float rotationSpeed = 720f;
	[SerializeField]
	float airRotationSpeed = 240f;
	[SerializeField]
    GameObject player = default;
    Movement sphere; 
    Rigidbody body;
    Vector3 DummyGrav;
    bool Gate = true;
    bool gravSwap;
	// Start is called before the first frame update
	public void SnapRotationToDirection(){
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(point.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
		Vector3 gravity = CustomGravity.GetUpAxis(this.transform.position);
		Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, 99999f * Time.deltaTime);
	}
    void Start()
    {
		transform.rotation = Quaternion.LookRotation( transform.forward , CustomGravity.GetUpAxis(transform.position));
        sphere = player.GetComponent<Movement>();
        body = player.GetComponent<Rigidbody>();
    }

    void Update() {
        UpdateSpins();
    }

	void gravFlip(){
            Quaternion toRotation = Quaternion.LookRotation( transform.forward , CustomGravity.GetUpAxis(this.transform.position) );
			this.transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, (rotationSpeed) * Time.deltaTime);
			Gate = true;
	}
    void UpdateSpins()
    {
		Vector3 player2Pointer = sphere.ProjectDirectionOnPlane(point.transform.position - transform.parent.gameObject.transform.position, CustomGravity.GetUpAxis(transform.position));
        Vector3 gravity = CustomGravity.GetUpAxis(this.transform.position);
		if(DummyGrav == gravity){
			if(Gate){
				gravSwap = false;
			}
		}
		else{
			gravSwap = true;
			DummyGrav = gravity;
			Gate = false;
		}
		if (gravSwap){
				Invoke("gravFlip", .6f);
		}
		else if (sphere.velocity.magnitude > .2f && (sphere.playerInput != Vector3.zero)){
			if(!sphere.OnGround){
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, airRotationSpeed * Time.deltaTime);
			}
			else{
				Quaternion toRotation = Quaternion.LookRotation(ProjectDirectionOnPlane(player2Pointer, gravity), gravity);
				transform.rotation = Quaternion.RotateTowards (transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
			}
		}
	}
    Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}
