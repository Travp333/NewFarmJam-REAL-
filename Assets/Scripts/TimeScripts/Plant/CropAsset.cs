using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "Crop Asset", menuName = "Scriptables/CropAsset", order = 1)]
public class CropAsset : ScriptableObject
{
    public GrowableCrop.cropType type;
    public GameObject prefab;
    
}
