using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
	[Header("File Storage Config")]
	[SerializeField] private string fileName;
    private SaveData saveData;
	private List<SaveInterface> SavingObjects;
	private FileDataHandler dataHandler;
    public static SaveManager instance { get; private set; }
	private void Awake()
	{
		if (instance != null) 
		{
			Debug.LogError("Found an extra save manager");
		}
		
		instance = this;
	}
	private void Start()
	{
		this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
		this.SavingObjects = FindAllSavingObjects();
		LoadGame();
	}

	public void NewGame() {
		this.saveData = new SaveData();
	}
	
	public void LoadGame() {

		this.saveData = dataHandler.Load();
		if (this.saveData == null) {
			Debug.Log("No data was found. New Game!");
			NewGame();
		}
		foreach (SaveInterface savingObject in SavingObjects) {
			savingObject.LoadData(saveData);
		}
	}
	public void SaveGame() {
		foreach (SaveInterface savingObject in SavingObjects) {
			savingObject.SaveData(ref saveData);
		}
		
		dataHandler.Save(saveData);
	}
	
	private void OnApplicationQuit() {
		SaveGame();
	}
	private List<SaveInterface> FindAllSavingObjects() {
		IEnumerable<SaveInterface> savingObjects = FindObjectsOfType<MonoBehaviour>().OfType<SaveInterface>();
		return new List<SaveInterface>(savingObjects);
	}
}
