using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowableCrop : Growable
{
    public enum cropType {TEST,TOAST};
    public cropType type;
    public CropAsset cropAsset;
    public GrowableCrop() {
        type = 0;
        
    }

}
