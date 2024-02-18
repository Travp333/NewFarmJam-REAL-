using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        if(other.gameObject.tag == "Player"){
            DropMe();
        }
    }
    public void DropMe(){
        Debug.Log("TESTING@!!@!!");
        inven.DropWholeStack(row.ToString() +", "+ column.ToString());
        inven.UIPlugger.GetComponent<UiPlugger>().ClearWorldModel(row, column);
        inven.UIPlugger.GetComponent<UiPlugger>().ClearSlot(row, column, inven.temp.emptyImage);
    }
}
