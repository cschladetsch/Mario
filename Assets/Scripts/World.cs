using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flow;
using UnityEngine;

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

	private int _beginLevel;

	public IKernel Kernel;

	void Awake()
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
		Debug.Log("Canvas " + Canvas.name);

		_levelIndex = 0;
		_areaIndex = 1;
	}

	void Start()
	{
		//Startlevel();
		//Pause(true);

		Debug.Log("World.Start");
		Kernel.Factory.NewCoroutine(TestCoro);

		var root = transform.FindChild("Areas");

		foreach (var a in AreaPrefabs)
		{
			var area = ((GameObject) Instantiate(a)).GetComponent<AreaBase>();
			var name = area.name.Replace("(Clone)", "") + "UI";
			var child = Canvas.transform.FindChild(name);
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

		BeginArea(1);
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
				ConveyorGame();		// conveyor game. deliver ingredients and products past boss
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
			Areas[n].gameObject.SetActive(act);
			Areas[n].UiCanvas.SetActive(act);
		}
	}

	private void Cooking()
	{
	}

	private void WaitingForTruck()
	{
		Debug.Log("WaitingForTruck");
	}

	private void MainShop()
	{

	}

	private void ConveyorGame()
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
		_areaIndex = 0;

		Reset();

		//BeginConveyorLevel();
	}

	void Update()
	{
		// need to wait a few updates before beginning, because we can have nested SpawnGameObject components...
		if (_beginLevel > 0)
		{
			--_beginLevel;

			if (_beginLevel == 0)
			{
				BeginConveyorLevel();

				// if this is the first level, then we pause else we un-pause
				Pause(_levelIndex == 0);
			}

			return;
		}
		
		if (_first)
		{
			_first = false;

			BeginArea(0);

			//Pause(true);
		}
	}

	public void Pause(bool pause)
	{
		if (pause == _paused)
			return;

		_paused = pause;

		foreach (var cake in FindObjectsOfType<Cake>())
			cake.Pause(pause);

		if (Level)
			Level.Pause(pause);
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

		Debug.Log("World.CreateLevel: " + Level.name);

		// actually begin the level after a few Updates to allow nested spawners to complete
		_beginLevel = 5;
	}

	public void BeginConveyorLevel()
	{
		Truck = FindObjectOfType<Truck>();

		Reset();

		Pause(false);

		Level.BeginLevel();
		Level.Pause(false);
	}

	public void NextLevel()
	{
		//Debug.Log("Next Level");
		_levelIndex = (_levelIndex + 1)%Levels.Length;
		_areaIndex = 2;
		CreateLevel();
	}

	public void NextArea()
	{
		Debug.Log("World.Next Area");
		_areaIndex = (_areaIndex + 1)%4;
		BeginArea(_areaIndex);
	}
}
