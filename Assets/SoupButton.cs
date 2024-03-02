using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupButton : MonoBehaviour
{
    [SerializeField]
    public Item soupitem;

    Inven playerInventory;
    Inven soupInventory;
    public void SoupClick() {
        Debug.Log("SFX CRAFT SOUP NOISE");
        
        foreach (Inven i in GameObject.FindObjectsOfType<Inven>())
        {
            if (i.gameObject.tag == "Player")
            {
                playerInventory = i;
            }
            if (i.gameObject.tag == "Cooker")
            {
                soupInventory = i;
            }
        }

        if (playerInventory != null && soupInventory != null)
        {
            int veggieCount = 0;


            foreach (ItemStat i in soupInventory.array)
            {
                if (i.isVeggie)
                {
                    veggieCount++;
                    Debug.Log(veggieCount + " veggies detected");
                }
                else
                    Debug.Log("Wrong Recipe");
            }


            if (veggieCount  == 3)
            {
                
                playerInventory.SmartPickUp(soupitem);

                //deduct
                for (int i = 0; i < soupInventory.vSize; i++)
                {
                    for (int i2 = 0; i2 < soupInventory.hSize; i2++)
                    {
                        if (soupInventory.array[i, i2].isVeggie)
                        {
                            soupInventory.DeductOneFromSlot(new string(i + "," + i2));
                        }
                    }
                }
            }
        }
        else Debug.Log("Soup Inventories are null");
    }
    
}
