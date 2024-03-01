using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainSch : MonoBehaviour
{
    [Tooltip("Every x amount of days")]
    [SerializeField, Range(1,10)]
    int interval = 3;

    [Tooltip("Hours the train stays")]
    [SerializeField]
    int lengthOfStay = 6;

    [Tooltip("Arrival Time")]
    [SerializeField]
    int arrivalOffset = 12;
    [SerializeField]
    GameClock clock;
    Animator anim;

    bool isArriving =false , isStopped = false, isDeparting = false, isGone =true;

    public bool arriveAnimComplete = false, departAnimComplete = false;

    //This script is enabled by the clock script every hour
    //It checks what it needs to do based on the clock and then disables itself
	private void OnEnable()
	{

        anim = GetComponent<Animator>();
        int ihours = interval * 24;
        int gameHour = clock.gameHour - arrivalOffset;
        int arrivalHour = 0;
        anim.SetBool("Done Shopping", false);
        anim.SetBool("Go", false);

        if ((gameHour) % ihours == 0 && isGone) {
            //start arriving
           
            anim.SetBool("Go", true);
            isArriving = true;
            isGone = false;
            arrivalHour = gameHour;
            
        }

        
        //stay for lengthOfStay
        if (gameHour% lengthOfStay == 0 && arriveAnimComplete && gameHour != arrivalHour) {
            
            anim.SetBool("Done Shopping", true);
            arriveAnimComplete = false;
            arrivalHour = gameHour;
            Debug.Log("Left Station at " + arrivalHour);
        }
        
        //end of departure
        if (isDeparting && departAnimComplete) {
            
            isGone = true;
            
        }
        
        Debug.Log(trainState());
        this.enabled = false;
	}
    private void StopTrain()
	{
        anim.SetBool("Done Shopping", false);
        anim.SetBool("Go", false);
        isStopped = true;
       
    }
    private void ArrivalComplete() {
        
        arriveAnimComplete = true;
        Debug.Log("Arrived at " + clock.gameHour);
        
    }
    private void DepartureComplete()
    {
        
        departAnimComplete = true;
        isDeparting = true;
        this.enabled = true;
    }


    private void CleanBools() {
        isStopped = isGone = isDeparting = isArriving = false;
        
    }
    private string trainState() {
        string s = "";
        if (isGone)
            s += "Train is Gone";
        if (isStopped)
            s += "Train is Stopped";
        if (isDeparting)
            s += "Train is Departing";
        if (isArriving)
            s += "Train is Arriving";
        return s;
    }
}
