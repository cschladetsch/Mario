﻿using System.Collections;
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
	public GameObject IngredientDetails;

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

	public Dictionary<AreaType, AreaBase> Areas = new Dictionary<AreaType, AreaBase>();

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

	private AreaType _areaType;

	public int GoalIndex;

	private bool _first = true;

	private int _beginLevelAfterThisManyUpdates;

	public IKernel Kernel;

	void Awake()
	{
		Awaken();
	}

	public ButtonsPanel Buttons;

	public void Awaken()
	{
		if (Instance != null)
			return;

		Kernel = Flow.Create.NewKernel();

		Debug.Log("World.Awaken: " + Kernel);
		if (Instance != null)
		{
			Debug.LogError("Can't have multiple Worlds");
			return;
		}

		Application.targetFrameRate = 60;

		Instance = this;

		Canvas = FindObjectOfType<UiCanvas>();

		_levelIndex = 0;
		_areaType = AreaType.Shop;

		GatherIngredients();
	}
	
	private void GatherIngredients()
	{
		//Debug.Log("World.GatherIngredients");

		foreach (Transform tr in IngredientDetails.transform)
		{
			var info = tr.GetComponent<IngredientInfo>();
			IngredientInfo.Add(info.Type, info);
			//Debug.Log("Adding info about " + info.Type);
		}
	}

	void Start()
	{
		Kernel.Factory.NewCoroutine(TestCoro);

		var root = transform.FindChild("Areas");

		foreach (var a in AreaPrefabs)
		{
			//Debug.Log("Creating a " + a.name);

			var area = ((GameObject) Instantiate(a)).GetComponent<AreaBase>();
			if (area == null)
			{
				Debug.LogError("Area prefab " + a.name + " doesn't have an AreaBase component");
				continue;
			}

			var name = area.name.Replace("(Clone)", "") + "UI";
			var child = Canvas.transform.FindChild(name);
			if (child == null)
			{
				Debug.LogWarning("Couldn't find UI for area " + area.name);
				continue;
			}

			area.UiCanvas = child.gameObject;
			area.transform.parent = root.transform;

			Areas[area.Type] = area;
		}

		Player = FindObjectOfType<Player>();

		GoalIndex = 0;
		Player.SetGoal(StageGoals[GoalIndex]);

		ChangeArea(_areaType);
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

	public void ChangeArea(AreaType area)
	{
		_areaType = area;

		CurrentArea = Areas[_areaType];

		if (!Areas.ContainsKey(_areaType))
		{
			Debug.LogWarning("World doesn't know about area " + _areaType);
			return;
		}

		switch (area)
		{
			case AreaType.Shop:
				MainShop();			// sending through stock, paying customers, and ordering new ingredients
				break;

			case AreaType.Factory:
				PlayConeyorGame();		// conveyor game. deliver ingredients and products past boss
				break;

			case AreaType.Bakery:
				EnterBakery();			// bakery: produce goods for selling in MainShop
				break;
		}

		DisableOtherAreas();

		CurrentArea.EnterArea();
	}

	private void DisableOtherAreas()
	{
		foreach (var kv in Areas)
		{
			var act = kv.Key == _areaType;
			var area = kv.Value;

			//Debug.Log("World.DisableOtherAreas: " + " act=" + act + " area=" + area);

			if (!act)
				area.LeaveArea();

			area.gameObject.SetActive(act);
			area.UiCanvas.SetActive(act);
			area.UiCanvas.BroadcastMessage("Reset", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void EnterBakery()
	{
		//if (CurrentLevel)
		//	Destroy(CurrentLevel.gameObject);
		
		//foreach (var c in FindObjectsOfType<Cake>())
		//	Destroy(c.gameObject);

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

	public void Restart()
	{
		_levelIndex = 0;

		Reset();

		BeginConveyorLevel();
	}

	void Update()
	{
		Kernel.Step();

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

	public void PlayConeyorGame()
	{
		//Debug.LogWarning("PlayConeyorGame");

		if (CurrentLevel)
		{
			//Debug.Log("PlayConeyorGame: CurrentLevel: " + CurrentLevel);
			//Debug.Log("PlayConeyorGame: CurrentLevel.Area: " + CurrentLevel.Area);
			return;

			//Debug.Log("Destroying " + CurrentLevel.name);
			//Destroy(CurrentLevel.gameObject);
		}

		var prefab = Levels[_levelIndex];
		CurrentLevel = ((GameObject)Instantiate(prefab)).GetComponent<Level>();
		CurrentLevel.transform.position = Vector3.zero;
		CurrentLevel.Area = CurrentArea as FactoryArea;

		CurrentLevel.Paused = true;

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

		PlayConeyorGame();

		ChangeArea(AreaType.Factory);
	}



	public void NextGoal()
	{
		GoalIndex = (GoalIndex + 1)%StageGoals.Length;
		Player.SetGoal(StageGoals[GoalIndex]);
	}

	public void MoveTo(AreaType area)
	{
		//Debug.Log("World.MoveTo: " + area);
		ChangeArea(area);
	}
}
