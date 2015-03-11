using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

// var is not used (right now!)
#pragma warning disable 649

/// <summary>
/// The overall controller for the game
/// </summary>
public class World : MonoBehaviour
{
	public int AreaIndex;

	public StageGoal []StageGoals;

	public AreaBase CurrentArea;

	public Level CurrentLevel;

	public List<Product> AvailableProducts;

	public Dictionary<IngredientType, IngredientInfo> IngredientInfo = new Dictionary<IngredientType, IngredientInfo>();

	public CookingAreaUI CookingAreaUi;

	public BuyingAreaUI BuyingAreaUi;


	/// <summary>
	/// The current level
	/// </summary>
	public GameObject[] Levels;

	public GameObject[] AreaPrefabs;

	public List<AreaBase> Areas = new List<AreaBase>();

	/// <summary>
	/// The single world instance
	/// </summary>
	public static World Instance;

	/// <summary>
	/// Current player
	/// </summary>
	public static Player Player;

	public static UiCanvas Canvas;

	/// <summary>
	/// The single truck
	/// </summary>
	public static Truck Truck;

	private bool _paused;

	private int _levelIndex;

	private int _areaIndex;

	public int GoalIndex;

	private bool _first = true;

	private int _beginLevelAfterThisManyUpdates;

	public IKernel Kernel;

	private void Awake()
	{
		Kernel = GetComponent<Kernel>().Kern;

		if (Instance != null)
		{
			Debug.LogError("Can't have multiple Worlds");
			return;
		}

		Application.targetFrameRate = 60;

		Instance = this;

		Canvas = FindObjectOfType<UiCanvas>();

		_levelIndex = 0;

		GatherIngredients();
	}
	private void GatherIngredients()
	{
		var infos = GameObject.Find("IngredientInfos");
		foreach (Transform tr in infos.transform)
		{
			var info = tr.GetComponent<IngredientInfo>();
			IngredientInfo.Add(info.Type, info);
		}
	}

	void Start()
	{
		Kernel.Factory.NewCoroutine(TestCoro);

		var root = transform.FindChild("Areas");

		foreach (var a in AreaPrefabs)
		{
			var area = ((GameObject) Instantiate(a)).GetComponent<AreaBase>();
			var name = area.name.Replace("(Clone)", "") + "UI";
			var child = Canvas.transform.FindChild(name);
			if (child == null)
			{
				Debug.LogWarning("Couldn't find UI for area " + area.name);
			}
			area.UiCanvas = child.gameObject;
			area.transform.parent = root.transform;
			Areas.Add(area);
		}

		Player = FindObjectOfType<Player>();

		GoalIndex = 0;
		Player.SetGoal(StageGoals[GoalIndex]);

		BeginArea(_areaIndex);
	}

	private IEnumerator TestCoro(IGenerator t0)
	{
		for (var n = 0; n < 100; ++n)
		{
			//Debug.Log("TestCoro " + n);
			yield return 0;
		}
	}

	public void Reset()
	{
		if (CurrentLevel == null)
			return;
	}

	public void BeginArea(int num)
	{
		if (CurrentArea)
			CurrentArea.End();

		_areaIndex = num;

		switch (num)
		{
			case 0:
				MainShop();			// sending through stock, paying customers, and ordering new ingredients
				break;
			case 1:
				WaitingForTruck();	// paying customers, waiting for truck
				break;
			case 2:
				CreateLevel();		// conveyor game. deliver ingredients and products past boss
				break;
			case 3:
				Cooking();			// bakery: produce goods for selling in MainShop
				break;
		}

		CurrentArea = Areas[_areaIndex];

		Debug.Log("New Area: " + CurrentArea.name);

		DisableOtherAreas();

		CurrentArea.StartArea();
	}

	private void DisableOtherAreas()
	{
		for (var n = 0; n < Areas.Count; ++n)
		{
			var act = n == _areaIndex;
			var area = Areas[n];

			if (!act)
			{
				area.EndArea();
				area.End();
			}

			area.gameObject.SetActive(act);
			area.UiCanvas.SetActive(act);
			area.UiCanvas.BroadcastMessage("Reset", SendMessageOptions.DontRequireReceiver);

			//Debug.Log("Area " + area.name + " enabled: " + act);
			//Debug.Log("AreaUI " + area.UiCanvas.name + " enabled: " + act);
		}
	}

	private void Cooking()
	{
		if (CurrentLevel)
			Destroy(CurrentLevel.gameObject);
		
		foreach (var c in FindObjectsOfType<Cake>())
			Destroy(c.gameObject);

		// HACKS
		//Camera.main.transform.position = new Vector3(0.2f, -0.8f, -20);
	}

	private void WaitingForTruck()
	{
		//Debug.Log("WaitingForTruck");
	}

	private void MainShop()
	{

	}

	public void ConveyorGame()
	{
		Truck.Reset();

		CurrentLevel.Reset();

		Player.Reset();

		foreach (var c in FindObjectsOfType<Cake>())
			Destroy(c.gameObject);
	}

	public void Restart()
	{
		_levelIndex = 0;

		Reset();

		BeginConveyorLevel();
	}

	void Update()
	{
		// need to wait a few updates before beginning, because we can have nested SpawnGameObject components...
		if (_beginLevelAfterThisManyUpdates > 0)
		{
			--_beginLevelAfterThisManyUpdates;

			if (_beginLevelAfterThisManyUpdates == 0)
			{
				//Debug.Log("BeginConveyorLevel");

				BeginConveyorLevel();

				// if this is the first level, then we pause else we un-pause
				Pause(_levelIndex == 0);

				CurrentLevel.BeginLevel();
			}

			return;
		}
		
		if (_first)
		{
			_first = false;

			Pause(true);
		}
	}

	public void Pause(bool pause)
	{
		//_paused = pause;

		//foreach (var cake in FindObjectsOfType<Cake>())
		//	cake.Pause(pause);

		//if (CurrentLevel)
		//	CurrentLevel.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}

	public void CreateLevel()
	{
		if (CurrentLevel)
			Destroy(CurrentLevel.gameObject);

		var prefab = Levels[_levelIndex];
		CurrentLevel = ((GameObject)Instantiate(prefab)).GetComponent<Level>();
		CurrentLevel.transform.position = Vector3.zero;

		CurrentLevel.Paused = true;

		AddSpawners();

		// actually begin the level after a few Updates to allow nested spawners to complete
		_beginLevelAfterThisManyUpdates = 5;
	}

	public void BeginConveyorLevel()
	{
		Truck = FindObjectOfType<Truck>();

		Reset();

		Pause(false);

		CurrentLevel.Pause(false);
	}

	private Dictionary<IngredientType, int> _contents;

	public void BeginMainGame(Dictionary<IngredientType, int> contents)
	{
		_contents = contents;

		_levelIndex = 0;

		CreateLevel();

		BeginArea(2);
	}

	/// <summary>
	/// Add spawners for contents of truck. TODO: move to CurrentLevel.cs
	/// </summary>
	private void AddSpawners()
	{
		// create spawners from what was in truck
		var types = new List<IngredientType>();
		
		foreach (var c in _contents)
		{
			if (c.Value == 0)
				continue;

			var type = c.Key;

			if (types.Contains(type))
				continue;

			types.Add(type);

			var sp = CurrentLevel.gameObject.AddComponent<SpawnInfo>();
			var info = IngredientInfo[type];
			sp.MinSpawnTime = info.MinSpawnRate;
			sp.MaxSpawnTime = info.MaxSpawnRate;
			sp.Weight = 1;
			sp.MaxSpawns = c.Value;
			sp.Type = type;

			// load prefabs to make ingredients from resources path
			var path = string.Format("{0}", type);
			var ob = Resources.Load(path);
			if (ob == null)
			{
				Debug.LogWarning("No prefab for ingredient " + type);
				continue;
			}

			sp.Prefab = (GameObject)ob;
			if (sp.Prefab == null)
			{
				Debug.LogWarning("Can't make a " + type  + ", using path " + path);
				continue;
			}

			//Debug.Log("Using " + sp.Prefab.name + " prefab to make " + c.Key);
		}

		CurrentLevel.Inventory = _contents;
	}

	public void NextGoal()
	{
		GoalIndex = (GoalIndex + 1)%StageGoals.Length;
		Player.SetGoal(StageGoals[GoalIndex]);
	}
}
