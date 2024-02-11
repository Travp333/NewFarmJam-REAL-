using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField]
	Transform destination;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	protected void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player"){
			Debug.Log("TEST");
			if(other.gameObject.GetComponent<RootReferenceHolder>()!=null){
				other.gameObject.GetComponent<RootReferenceHolder>().root.transform.position = destination.position;
			}
			
		}
	}
}
