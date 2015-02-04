using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : HasWorld
{
	public GameObject CakePrefab;

	public GameObject CakeSpawnPoint;

	public float MinSpawnTime = 3;

	public float MaxSpawnTime = 6;

	/// <summary>
	/// How long before next speed level increase
	/// </summary>
	public float SpeedIncrementTime = 30;

	/// <summary>
	/// How much to speed up spawn times at each speed level increase
	/// </summary>
	public float SpawnIncrementRate = 1.3f;

	/// <summary>
	/// How much to speed up conveyor belt speed at each speed level increase
	/// </summary>
	public float SpeedIncrementRate = 1.2f;

	/// <summary>
	/// Default conveyor belt speed
	/// </summary>
	public float ConveyorSpeed = 0.3f;

	float OverallSpeed = 1;

	List<Conveyor> _conveyors = new List<Conveyor>();

	private float _spawnTimer;

	private Transform _cakesHolder;

	private float _speedLevel;

	Character []_characters;

	private float _initialConveyorSpeed;

	protected override void Construct()
	{
		_initialConveyorSpeed = ConveyorSpeed;
		base.Construct();
		_characters = FindObjectsOfType<Character>();

		foreach (var ch in _characters)
			ch.Pause(true);
	}

	public void BeginLevel()
	{
		Debug.Log("Level begins");

		Reset();

		_cakesHolder = transform.FindChild("Cakes");

		Player.Reset();

		foreach (var ch in _characters)
			ch.Pause(false);
	}

	public void Reset()
	{
		GatherConveyors();

		ConveyorSpeed = _initialConveyorSpeed;
		foreach (var c in _conveyors)
		{
			c.Reset();
			c.Speed = _initialConveyorSpeed;
		}

		_speedLevel = 1;
		_speedTimer = SpeedIncrementTime;
		OverallSpeed = 1;
	}

	private void GatherConveyors()
	{
		_conveyors.Clear();

		var root = transform.FindChild("Conveyors");
		_conveyors = root.GetComponentsInChildren<Conveyor>().ToList();
		_conveyors.Sort((a, b) => System.String.Compare(a.name, b.name, System.StringComparison.Ordinal));

		foreach (var c in _conveyors)
			c.Speed = ConveyorSpeed;
	}

	private float _speedTimer;

	void Update()
	{
		UpdateSpeed();

		if (Input.GetKeyDown(KeyCode.Return))
		{
			var cake = NewCake();
			cake.transform.position = CakeSpawnPoint.transform.position;
			var truck = FindObjectOfType<Truck>();
			truck.AddCake(cake.GetComponent<Cake>());
		}

		if (Paused)
			return;

		SpawnCake();
	}

	private void UpdateSpeed()
	{
		_speedTimer -= Time.deltaTime;
		if (!(_speedTimer < 0)) 
			return;

		_speedTimer = SpeedIncrementTime;
		OverallSpeed *= 1.0f/SpawnIncrementRate;

		foreach (var c in _conveyors)
			c.Speed *= SpeedIncrementRate;

		//Debug.Log("Speed Up " + _speedLevel);

		_speedLevel++;
	}

	private void SpawnCake()
	{
		_spawnTimer -= Time.deltaTime;
		if (_spawnTimer > 0)
			return;

		_spawnTimer = OverallSpeed*UnityEngine.Random.Range(MinSpawnTime, MaxSpawnTime);

		var cake = NewCake();
		var value = UnityEngine.Random.value;
		var n = (int) (value*100000);
		cake.name = n.ToString();

		cake.transform.position = CakeSpawnPoint.transform.position;
	}

	private GameObject NewCake()
	{
		var cake = (GameObject)Instantiate(CakePrefab);
		cake.transform.parent = _cakesHolder;
		return cake;
	}

	public Conveyor GetConveyor(int height, bool right)
	{
		var c = GetConveyor(height);
		if (!c)
			return null;

		if (right && !c.MoveRight)
			return null;

		return c;
	}

	public Conveyor GetConveyor(int height)
	{
		return height >= _conveyors.Count ? null : _conveyors[height];
	}

	public void Pause(bool pause)
	{
		Paused = pause;

		foreach (var conv in _conveyors)
			conv.Pause(pause);
	}
}
