using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class pickUpableItem : MonoBehaviour
{
    [SerializeField]
    public Inven inven;
    [SerializeField]
    public Item item;
    [SerializeField]
    public int row;
    [SerializeField]
    public int column;

    void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.tag == "Player"){
        //    DropMe();
        //}
    }
    public void DropMe(){
        //Debug.Log("TESTING@!!@!!");

        if(inven != null){
            inven.DropWholeStack(row.ToString() +", "+ column.ToString());
            inven.UIPlugger.GetComponent<UiPlugger>().ClearWorldModel(row, column);
            inven.UIPlugger.GetComponent<UiPlugger>().ClearSlot(row, column, inven.temp.emptyImage);
            for (int i = 0; i < item.harvestCount; i++)
            {
                GameObject.Find("3rd Person Character").GetComponent<Inven>().SmartPickUp(item.harvestsInto);
            }
        }
        else{
            //Debug.Log("TESTTTTT!!!!!");
            for (int i = 0; i < item.harvestCount; i++)
            {
                GameObject.Find("3rd Person Character").GetComponent<Inven>().SmartPickUp(item.harvestsInto);
            }
        }
        
    }
}
