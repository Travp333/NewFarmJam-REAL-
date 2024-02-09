using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour, SaveInterface
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightPreset preset;
    [SerializeField] public GrowingManager growingManager;

    [SerializeField, Range(0f, 480f)]
    
    float gameTimeScale = 360f; //360 is 10 gamedays per real hour
    float sessionPlayTime = 0f;
    float totalPlayTime = 0f;
    public int gameHour = 0;

	private void Start()
	{
        gameTimeScale = gameTimeScale / 3600f; //converts scaling from seconds to hours. 
                                               //moved from update to save calculations
        
	}
    int gameHourLastFrame = 0;
	void Update()
    {
        sessionPlayTime += Time.deltaTime;
        float gameTimeinHours = (sessionPlayTime + totalPlayTime) * gameTimeScale;
        gameHour = Mathf.FloorToInt(gameTimeinHours);
        if (gameHour - gameHourLastFrame > 0) 
        {
            NewHour();
        }

        gameHourLastFrame = gameHour;

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Debug.Log(("current session length " + (sessionPlayTime / 60f)) + " minutes, " + "Total game time" + gameTimeinHours + " in hours" ); 
        }
    }

    public void LoadData(SaveData data) {
        this.gameHour = data.gameHour;
        this.totalPlayTime = data.totalPlayTime;
        
    }
    public void SaveData(ref SaveData data) {
        data.gameHour = this.gameHour;
        data.totalPlayTime += this.sessionPlayTime;


    }
    public void NewHour() {
        Debug.Log("New Hour: " + gameHour);
        UpdateLighting();
        if(growingManager != null)
        growingManager.GrowStepUpdate();
    }
	private void OnValidate()
	{
        if (DirectionalLight != null)
            return;
        if (RenderSettings.sun != null) {
            DirectionalLight = RenderSettings.sun;
        }
	}
	void UpdateLighting() {
        float timepercent = (gameHour % 24f) / 24f;

        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timepercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timepercent);
        Debug.Log("Fog Color " + RenderSettings.fogColor +" at " + timepercent );
        DirectionalLight.color = preset.DirectionalColor.Evaluate(timepercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(((gameHour % 24) * 15) - 90f, 170f, 0)); 

    }
}
