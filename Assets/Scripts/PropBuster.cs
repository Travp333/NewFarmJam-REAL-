using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Breaks or damages any breakable object this object collides with
//Travis Parks
public class PropBuster : MonoBehaviour
{
    //[SerializeField]
    //AudioSource[] punchSounds;
    [SerializeField]
    float power;
    [SerializeField]
    float radius;
    [SerializeField]
    bool oneShot = false;
    Shatter otherExplosive;
    void OnCollisionEnter(Collision other) {

        //if(other.gameObject.tag != "Player" && punchSounds.Length != 0){
        //    int index = Random.Range(0, punchSounds.Length - 1);
        //    punchSounds[index].Play();
        //}

	    if(other.gameObject.GetComponent<Rigidbody>() != null){
            if (other.gameObject.GetComponent<Rigidbody>() != null && other.gameObject.tag != "Breakable" || other.gameObject.tag != "Explosive"){
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(power, transform.root.position, radius);
            }
            if (other.gameObject.tag == "Breakable" || other.gameObject.tag == "Explosive"){
                if(!oneShot){
                    otherExplosive = other.gameObject.GetComponent<Shatter>();
                    other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(power, transform.root.position, radius);
                    otherExplosive.takeDamage();
                }
                else{
                	Debug.Log("Breaking object!");
                	otherExplosive = other.gameObject.GetComponent<Shatter>();
                	otherExplosive.oneShot(0);
                }
            }
        }
    }
}

