using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SaveInterface 
{
    void LoadData(SaveData data);

    void SaveData(ref SaveData data);
}
