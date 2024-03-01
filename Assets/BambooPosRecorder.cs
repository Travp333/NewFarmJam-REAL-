using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooPosRecorder : MonoBehaviour
{
    [SerializeField]
    int maxBamboos = 100;
    [SerializeField]
    GameObject[] Bamboos;
    public static List<Vector3> bambooSpawnPositions = new List<Vector3>();
    public static List<Vector3> bambooSpawnPositionsClone = new List<Vector3>();
    [SerializeField]
    public int recordIncrement = 5;
    public Transform player;
    [SerializeField]
    public List<GameObject> spawnedBamboo = new List<GameObject>();
    // Start is called before the first frame update
    [SerializeField]
    public float maxDistance = 5f;
    void Start()
    {
        player = GameObject.Find("3rd Person Character").GetComponent<Movement>().feet.transform;
    }
    public void spawnBamboo(){
        if(spawnedBamboo.Count > 0){
            foreach(GameObject b in spawnedBamboo){
                spawnedBamboo.Remove(b);
                Destroy(b);
            }
        }
        if(bambooSpawnPositions.Count > 0){
            foreach ( Vector3 t in bambooSpawnPositions){
                GameObject g =  Instantiate(Bamboos[Random.Range(0, Bamboos.Length)], t, Quaternion.identity);
                g.transform.localScale = g.transform.localScale * 26;
                spawnedBamboo.Add(g);
            }
        }
    }
    public void BambooPosRecord(){
        if(bambooSpawnPositions.Count > maxBamboos){
            if(bambooSpawnPositions.Count <=0 ){
                bambooSpawnPositions.Add(player.position);
                Debug.Log("Recording first Position! (somehow????)");
            }
            else{
                bambooSpawnPositions.Remove(bambooSpawnPositions[0]);
                foreach(Vector3 BS in bambooSpawnPositions){
                    if(Vector3.Distance(player.position, BS) > maxDistance){
                        bambooSpawnPositions.Add(player.position);
                        Debug.Log("Overwriting Position!");
                    }
                }
            }

        }
        else{
            if(bambooSpawnPositions.Count <=0 ){
                bambooSpawnPositions.Add(player.position);
                Debug.Log("Recording first Position!");
            }
            else{



                for (int index = 0; index < bambooSpawnPositions.Count; index++)
                {
                        if(Vector3.Distance(player.position, bambooSpawnPositions[index]) > maxDistance){
                            bambooSpawnPositions.Add(player.position);
                            Debug.Log("Recording Position!");
                            return;
                        }
                        else{
                            Debug.Log("Blocking too close seed?");

                        }
                }




               // bambooSpawnPositionsClone = bambooSpawnPositions;
               // foreach(Vector3 t in bambooSpawnPositionsClone){
               //     if(Vector3.Distance(player.position, t) > maxDistance){
                //        bambooSpawnPositions.Add(player.position);
                //        Debug.Log("Recording Position!");
               //     }
               //     else{
               //         Debug.Log("Blocking too close seed?");
               //     }
              //  }

            }

        }
    }

}
