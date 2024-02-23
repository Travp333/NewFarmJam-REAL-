using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour, SaveInterface
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightPreset preset;
    [SerializeField] public GrowingManager growingManager;

    
    [SerializeField]
    float gameTimeScale = 3600f; //3600 is 100 gamedays per real hour
    float sessionPlayTime = 0f;
    float totalPlayTime = 0f;
    float gameTimeInHours = 0f;
    public int gameHour = 0;

	private void Start()
	{
        gameTimeScale = gameTimeScale / 3600f; //converts scaling from seconds to hours. 
        UpdateLighting();                     //moved from update to save calculations
        
	}
    int gameHourLastFrame = 0;
	void Update()
    {
        sessionPlayTime += Time.deltaTime;
        gameTimeInHours = (sessionPlayTime + totalPlayTime) * gameTimeScale;
        gameHour = Mathf.FloorToInt(gameTimeInHours);
        //check when an has passed
        if (gameHour - gameHourLastFrame > 0) 
        {
            NewHour();
        }

        gameHourLastFrame = gameHour;
        UpdateLighting();

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
        float timepercent = (gameTimeInHours % 24f) / 24f;

        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timepercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timepercent);
        
        DirectionalLight.color = preset.DirectionalColor.Evaluate(timepercent);
        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(((timepercent) * 360) - 90f, 170f, 0)); 

    }
}
