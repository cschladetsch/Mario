﻿using System.Collections;
using System.Collections.Generic;
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

	public GameObject[] Areas;

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

		_levelIndex = 0;
		_areaIndex = 1;
	}

	void Start()
	{
		//Startlevel();
		//Pause(true);

		Kernel.Factory.NewCoroutine(TestCoro);
	}

	private IEnumerator TestCoro(IGenerator t0)
	{
		for (var n = 0; n < 100; ++n)
		{
			Debug.Log("TestCoro " + n);
			yield return 0;
		}
	}

	public void Reset()
	{
		if (Level == null)
			return;

		ResetArea(1);
	}

	void ResetArea(int num)
	{
		switch (num)
		{
			case 1:
				MainShop();
				break;
			case 2:
				WaitingForTruck();
				break;
			case 3:
				ConveyorGame();
				break;
			case 4:
				Cooking();
				break;
		}
	}

	private void Cooking()
	{
	}

	private void WaitingForTruck()
	{
		throw new System.NotImplementedException();
	}

	private void MainShop()
	{
		throw new System.NotImplementedException();
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

		Reset();

		BeginLevel();
	}

	void Update()
	{
		// need to wait a few updates before beginning, because we can have nested SpawnGameObject components...
		if (_beginLevel > 0)
		{
			--_beginLevel;

			if (_beginLevel == 0)
			{
				BeginLevel();

				// if this is the first level, then we pause else we unpause
				Pause(_levelIndex == 0);
			}

			return;
		}

		if (_first)
		{
			_first = false;

			CreateLevel();

			Pause(true);
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

	public void BeginLevel()
	{
		Player = FindObjectOfType<Player>();
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
		CreateLevel();
	}
}
