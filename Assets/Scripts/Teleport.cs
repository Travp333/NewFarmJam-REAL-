using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField]
	Transform destination;
	[SerializeField]
	bool isHomeTrigger;
	protected void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player"){
			if(other.gameObject.GetComponent<RootReferenceHolder>()!=null){
				other.gameObject.GetComponent<RootReferenceHolder>().root.transform.position = destination.position;
			}
			if(isHomeTrigger){
				GameObject.Find("Directional Light").GetComponent<BambooPosRecorder>().spawnBamboo();
			}
			
		}
	}
}
