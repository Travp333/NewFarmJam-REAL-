using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour
{
   [SerializeField] public float gameTime = 0f;

    [SerializeField, Range(24f, 480f)]
    [Tooltip("In-game hours per real hour")]
    float gameHourScale;
    float realTime = 0f;

	private void Start()
	{
		
	}
	void Update()
    {
        realTime += Time.deltaTime;
        gameTime += Time.deltaTime *gameHourScale/3600;
        float gameHour = gameTime%3600;
        if (Input.GetKeyDown(KeyCode.Space)) { Debug.Log(((realTime/60f)%60f) + " minutes playing, " + gameHour + "in game hour" ); }
    }
}
