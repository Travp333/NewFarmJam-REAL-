using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwap : MonoBehaviour
{
	[SerializeField]
	Camera switchToCam;
	Camera switchFromCam;
	Movement player;
	RotationPointer rotationPointer;
    // Start is called before the first frame update
    void Start()
    {
	    switchFromCam = GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera;
	    player = GameObject.Find("3rd Person Character").GetComponent<Movement>();
	    rotationPointer = GameObject.Find("RotationPointer").GetComponent<RotationPointer>();
    }

	protected void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player"){
			switchToCam.enabled = true;
			switchToCam.GetComponent<AudioListener>().enabled = true;
			switchFromCam.enabled = false;
			switchFromCam.GetComponent<AudioListener>().enabled = false;
			switchFromCam = GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera;
			GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera = switchToCam;
			player.playerInputSpace = switchToCam.transform;
			rotationPointer.playerinputSpace = switchToCam.transform;
		}
	}
	protected void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player"){
			
			switchToCam.enabled = false;
			switchToCam.GetComponent<AudioListener>().enabled = false;
			switchFromCam.enabled = true;
			switchFromCam.GetComponent<AudioListener>().enabled = true;
			player.playerInputSpace = switchFromCam.transform;
			rotationPointer.playerinputSpace = switchFromCam.transform;
			GameObject.Find("TopDownCam").GetComponent<ActiveCameraTracker>().activeCamera = switchFromCam;
		}
	}
}
