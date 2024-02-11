using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//keeps track of which direction the player is inputting in  
public class RotationPointer : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 10;
    [SerializeField]
    GameObject player = default;
    Movement sphere; 
    [SerializeField]
    public Transform playerinputSpace = default;
    // Start is called before the first frame update
    void Start()
    {
        sphere = player.GetComponent<Movement>();
    }

    // Update is called once per frame
	void Update () {
        transform.localPosition = (sphere.ProjectDirectionOnPlane(playerinputSpace.TransformDirection(sphere.playerInput.x, 0f, sphere.playerInput.y) * maxSpeed, CustomGravity.GetUpAxis(transform.position))*10f );
        
		//transform.localPosition = new Vector3(playerInput.x, 0.5f, playerInput.y);
	}
}
