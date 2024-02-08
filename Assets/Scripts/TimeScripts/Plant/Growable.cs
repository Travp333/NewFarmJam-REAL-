using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Growable 
{
    public int stepsToGrow;
    public int currentGrowthStep;
    public Vector3 position;

    public Growable() {
        stepsToGrow = 0;
        currentGrowthStep = 0;
        position = new Vector3(0f,0f,0f);
    }
    public void UpdateGrowth() {
        currentGrowthStep++;
        
    }

}
