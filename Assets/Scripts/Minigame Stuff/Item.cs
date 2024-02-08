﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script stores information about Inventory Objects
//Written by Travis
[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    public string Objname;
    [SerializeField]
    public int stackSize;
    [SerializeField]
	public Sprite img;
	[SerializeField]
	public bool requiresIngredient;
	[SerializeField]
	public string ingredientName;
}
