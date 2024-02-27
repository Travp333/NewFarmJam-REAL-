using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script stores information about Inventory Objects
//Written by Travis
[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
	[SerializeField]
	public Item me;
    [SerializeField]
    public string Objname;
    [SerializeField]
    public int stackSize;
    [SerializeField]
    public Sprite img;
    [SerializeField]
	public GameObject[] worldModel;
	[SerializeField]
	public Item requiredIngredient;
    [SerializeField]
    public Item craftsInto;
	[SerializeField]
	public Item growsInto;
	[SerializeField]
	public bool grabbable;
	[SerializeField]
	public bool isSeed;
    [SerializeField]
    public int age;
    [SerializeField]
    public int matureAge;
    [SerializeField]
    public Item harvestsInto;
    

}
