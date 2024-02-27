using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
//syncs the world mesh with the inner inventory system logic
public class InvenSyncer : MonoBehaviour
{
	[SerializeField]
	Transform[] cropPositions;
	// Start is called before the first frame update
	//should probably take an item input for what item and a atring input for what slot?
	public void UpdateWorldModel(int row, int column, string name, GameObject worldModel){
		//Debug.Log("Updating World Model!");
		foreach (Transform t in cropPositions){
			//Debug.Log("comparing "+ t.name + " to " + row + ","+ column);
			if (t.name ==  (row + ","+ column)){
				
				GameObject g = Instantiate(worldModel, t.transform.position, Quaternion.identity, t);
				pickUpableItem g2 = g.GetComponent<pickUpableItem>();
				g2.inven = this.gameObject.GetComponent<Inven>();
				g2.row = row;
				g2.column = column;
			}
		}
	}
	public void ClearWorldModel(int row, int column){
		foreach (Transform t in cropPositions){
			if (t.name ==  (row + ","+ column)){
				foreach(Transform child in t.transform)
				{
					Destroy(child.gameObject);
				}
			}
		}
	}
}
