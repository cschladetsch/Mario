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

	public AreaBase CurrentArea;

	public Level Level;

	public List<Product> AvailableProducts;

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

	private bool _first = true;

	private int _beginLevelAfterThisManyUpdates;

	public IKernel Kernel;

	private void Awake()
	{
		Kernel = FindObjectOfType<Kernel>().Kern;

		if (Instance != null)
		{
			Debug.LogError("Can't have multiple Worlds");
			return;
		}

		Application.targetFrameRate = 60;

		Instance = this;

		Canvas = FindObjectOfType<UiCanvas>();

		_levelIndex = 0;
	}

	void Start()
	{
		//Startlevel();
		//Pause(true);

		//Debug.Log("World.Start");
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

		BeginArea(0);
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
		if (Level == null)
			return;
	}

	public void BeginArea(int num)
	{
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

		//Debug.Log("Using Area " + _areaIndex + " from " + Areas.Count);
		CurrentArea = Areas[_areaIndex];

		DisableOtherAreas();
	}

	private void DisableOtherAreas()
	{
		for (var n = 0; n < Areas.Count; ++n)
		{
			var act = n == _areaIndex;
			var area = Areas[n];

			area.gameObject.SetActive(act);
			area.UiCanvas.SetActive(act);
		}
	}

	private void Cooking()
	{
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

		Level.Reset();

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
				Debug.Log("BeginConveyorLevel");

				BeginConveyorLevel();

				// if this is the first level, then we pause else we un-pause
				Pause(_levelIndex == 0);

				Level.BeginLevel();
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

		//if (Level)
		//	Level.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}

	public void CreateLevel()
	{
		if (Level)
			Destroy(Level.gameObject);

		var prefab = Levels[_levelIndex];
		Level = ((GameObject)Instantiate(prefab)).GetComponent<Level>();
		Level.transform.position = Vector3.zero;

		Level.Paused = true;

		AddSpawners();

		// actually begin the level after a few Updates to allow nested spawners to complete
		_beginLevelAfterThisManyUpdates = 5;
	}

	public void BeginConveyorLevel()
	{
		Truck = FindObjectOfType<Truck>();

		Reset();

		Pause(false);

		Level.Pause(false);
	}

	private Dictionary<Ingredient.TypeEnum, int> _contents;

	public void BeginMainGame(Dictionary<Ingredient.TypeEnum, int> contents)
	{
		_contents = contents;

		_levelIndex = 0;

		CreateLevel();

		BeginArea(2);
	}

	private void AddSpawners()
	{
// create spawners from what was in truck
		foreach (var c in _contents)
		{
			if (c.Value == 0)
				return;

			var sp = Level.gameObject.AddComponent<SpawnInfo>();
			sp.MinSpawnTime = 2;
			sp.MaxSpawnTime = 4;
			sp.Weight = 1;
			sp.MaxSpawns = c.Value;

			// load prefabs to make ingredients from resources path
			var path = string.Format("{0}", c.Key);
			var ob = Resources.Load(path);
			//Debug.Log("loaded '" + ob + "' from path " + "'" + path + "'");

			sp.Prefab = (GameObject) Instantiate(ob);
			if (sp.Prefab == null)
			{
				Debug.LogWarning("Can't make a " + c.Key  + ", using path " + path);
				continue;
			}

			//Debug.Log("Using " + sp.Prefab.name + " prefab to make " + c.Key);
		}
	}
}
