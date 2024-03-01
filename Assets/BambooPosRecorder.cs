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
    [SerializeField]
    public int recordIncrement = 5;
    public Transform player;
    [SerializeField]
    public List<GameObject> spawnedBamboo = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("3rd Person Character").GetComponent<Movement>().feet.transform;
    }
    public void spawnBamboo(){
        if(spawnedBamboo.Count > 0){
            foreach(GameObject b in spawnedBamboo){
                Destroy(b);
            }
            spawnedBamboo.Clear();
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
            bambooSpawnPositions.Remove(bambooSpawnPositions[0]);
            if(!bambooSpawnPositions.Contains(player.position)){
                bambooSpawnPositions.Add(player.position);
                //Debug.Log("Overwriting Position!");
            }

        }
        else{
            if(!bambooSpawnPositions.Contains(player.position)){
                bambooSpawnPositions.Add(player.position);
                //Debug.Log("Recording Position!");
            }
        }

        
    }

}
