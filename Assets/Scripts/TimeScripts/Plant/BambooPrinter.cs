using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BambooPrinter : MonoBehaviour
{
	
	float stepSize = 10f;
	int resolution = 128;
	float[,] heights;
	[SerializeField]
	RawImage output;
	[SerializeField]
	bool printHeight = false; //disables print function and messes up working values
	[SerializeField]
	GameObject endObj = null;
	public bool ScanSuccessful;

	[SerializeField]
	GameObject spawnPrefab;
	Vector3 startPos;
	
	private void Start()
	{

		if (endObj != null)
		{
			
			startPos = transform.position;

			float dx = Mathf.Abs(endObj.transform.position.x - transform.position.x);
			float dz = Mathf.Abs(endObj.transform.position.z - transform.position.z);
			if (dx >= dz) {
				stepSize = dx / resolution;
			}
			if (dx < dz) {
				stepSize = dz / resolution;
			}
			if (!ScanSuccessful)
			{
				ScanMap(resolution);
			}
			
		}
		

	}
	void ScanMap(int size) {
		heights = new float[size,size];
		Vector3 pv = transform.position;
		float height = -999f;


		RaycastHit hit;
		int missCount = 0;
		float minH = 999f, maxH = 0f;
		
		for (int x = 0; x < size; x++) {
			for (int z = 0; z < size; z++) {
				if (Physics.Raycast(pv, transform.TransformDirection(Vector3.down), out hit, 500f))
				{
					height = hit.point.y;
					
				}
				else
				{
					//couldnt hit ground
					
				}
				heights[x,z] = height;
				

				
				if (height >= 0f) //above sea level
				{
					
					missCount = 0;
					//getting possible range of values 
					if (minH > height) { //sets the land height range, if current mininum is greater than height, thats the new min
						minH = height;
					}
					if (maxH < height) {
						maxH = height;
					}
				}
				
				else
				{
					height = 0f;
					missCount++;
				}
				pv.Set(startPos.x + x * stepSize, startPos.y, startPos.z + z * stepSize);
				transform.position = pv;

				//if the raycast misses enough skip this line
				if (missCount > 64) {
					Debug.Log("missed 32 times");
					z = size;
					missCount = 0;
				}


			}
		}
		ScanSuccessful = true; //SCAN SUCCESS!
		Debug.Log("Scan Successful");
		Debug.Log(minH);
		Debug.Log(maxH);
		//scan loop end,sort loop start
		if (printHeight)
		{
			//normalize heights for height map
			float a = 1 / (maxH - minH);
			float b = -minH / (maxH - minH);
			Texture2D t = new Texture2D(resolution, resolution);

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					heights[x, y] *= a;
					heights[x, y] += b;

					t.SetPixel(x, y, new Color(heights[x, y], heights[x, y], heights[x, y], 1));
				}
			}
			t.Apply();
			output.texture = t;
		}
		else
			if (output != null)
		{
			output.enabled = false;
		}
	}
	public void PrintObjects(List<Vector3> customPositions) {
		Debug.Log("Printer called with "+ customPositions.Count);
		if (!ScanSuccessful) {
			Start();
		}
		
		foreach (Vector3 p in customPositions) {
				int x = (int)p.x;
				int z = (int)p.y;

				float yHeight = heights[x, z];

				Vector3 spawnPos = new Vector3(startPos.x + (x * stepSize), yHeight, startPos.z + (z * stepSize));
			Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f,360f));
			Instantiate(spawnPrefab, spawnPos, rot);
			Debug.Log("Tried to spawn at " + heights[x, z]) ;
			}
		
		
	}
	

}


	

