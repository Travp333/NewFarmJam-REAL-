using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
	[SerializeField]
	public GameObject shopMenu;

	private void Start()
	{
		if (shopMenu == null) {
			shopMenu = new GameObject();
		}
		shopMenu.SetActive(false);
	}
}
