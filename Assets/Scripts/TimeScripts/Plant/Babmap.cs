using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Babmap : MonoBehaviour
{
	[SerializeField]
	BambooPrinter printer;
	[SerializeField]
	public Texture2D texture;
	private void OnEnable()
	{
		List<Vector3> mapData = new List<Vector3>();
		if (printer == null)
		{
			Debug.Log("Printer disconnected");
			
		}
		else if (texture == null)
		{
			Debug.Log("Texture disconnected");
			
		}
		else {
			Debug.Log("Loop Started");




			for (int x = 0; x < 128; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					Color c = texture.GetPixel(x, y);
					if (c.grayscale > .5f)
					{
						Debug.Log("Found white pixel");
						mapData.Add(new Vector3(x, y, c.grayscale));
					}

				}
			}
			if (mapData.Count > 0) 
			printer.PrintObjects(mapData);
			else
				Debug.Log("Couldnt get mapData");
		}
			
		}
		
			
	
}
