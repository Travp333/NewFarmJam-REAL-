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

    bool isArriving =false , isStopped = false, isDeparting = false, isGone =true;

    public bool arriveAnimComplete = false, departAnimComplete = false;

    //This script is enabled by the clock script every hour
    //It checks what it needs to do based on the clock and then disables itself
	private void OnEnable()
	{
        int ihours = interval * 24;
        int gameHour = clock.gameHour + arrivalOffset;

        
        if ((gameHour) % ihours == 0) {
            //start arriving
            CleanBools();
            isArriving = true;
            
        }
        
        if (isArriving && arriveAnimComplete && !isStopped) {
            //stop the train
            StopTrain();
            isStopped = true;
        }
        //stay for lengthOfStay
        if (gameHour% ihours == lengthOfStay && isStopped) {
            CleanBools();
            isDeparting = true;
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
        CleanBools();
        isStopped = true;
        

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
