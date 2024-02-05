using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    bool freezeZXAxis;
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("TopDownCam").GetComponent<Camera>()!= null){
            cam = GameObject.Find("TopDownCam").GetComponent<Camera>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cam != null){
            if(freezeZXAxis){
                transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
            }
            else{
                transform.rotation = cam.transform.rotation;
            }
        }
    }
}
