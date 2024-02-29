using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCameraTracker : MonoBehaviour
{
	[SerializeField]
	public Camera activeCamera;
    // Start is called before the first frame update
    void Start()
    {
	    activeCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
