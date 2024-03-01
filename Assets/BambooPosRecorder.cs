using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooPosRecorder : MonoBehaviour
{
    [SerializeField]
    GameObject[] Bamboos;
    public static List<Transform> bambooSpawnPositions = new List<Transform>();
    [SerializeField]
    public int recordIncrement = 5;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("3rd Person Character").GetComponent<Movement>().feet.transform;
    }
    public void spawnBamboo(){
        foreach ( Transform t in bambooSpawnPositions){

            GameObject g =  Instantiate(Bamboos[Random.Range(0, Bamboos.Length)], player.position, Quaternion.identity);
            g.transform.localScale = g.transform.localScale * 26;
            
        }
    }
    public void BambooPosRecord(){
        bambooSpawnPositions.Add(player);
        Debug.Log("Recording Position!");
        
    }

}
