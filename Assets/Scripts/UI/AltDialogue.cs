using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AltDialogue 
{
    [SerializeField]
    public string name;
    [SerializeField, TextArea(3, 10)]
    public string[] sentences;
}
