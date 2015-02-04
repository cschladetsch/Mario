using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls aspects of the level, like the conveyors, characters, and cakes
/// </summary>
public class Level : HasWorld
{
	/// <summary>
	/// How to make a new cake
	/// </summary>
	public GameObject CakePrefab;

	/// <summary>
	/// Where to place newly made cake
	/// </summary>
	public GameObject CakeSpawnPoint;

	/// <summary>
	/// The shortest time that a cake may be spawned again
	/// </summary>
	public float MinSpawnTime = 3;

	/// <summary>
	/// The longest time that a cake may be spawned again
	/// </summary>
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

	public float SpeedLevel;

	public float OverallSpeed = 1;

	private List<Conveyor> _conveyors = new List<Conveyor>();
	private float _spawnTimer;
	private Transform _cakesHolder;
	private Character []_characters;
	private float _initialConveyorSpeed;

	protected override void Construct()
	{
		base.Construct();

		_initialConveyorSpeed = ConveyorSpeed;
		_characters = FindObjectsOfType<Character>();
		_cakesHolder = transform.FindChild("Cakes");

		PauseCharacters(true);
	}

	public void BeginLevel()
	{
		Debug.Log("Level begins");

		Reset();

		Player.Reset();

		PauseCharacters(false);
	}

	private void PauseCharacters(bool pause)
	{
		foreach (var ch in _characters)
			ch.Pause(pause);
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

		SpeedLevel = 1;
		_speedTimer = SpeedIncrementTime;

		OverallSpeed = 1;
	}

	private void GatherConveyors()
	{
		_conveyors.Clear();

		var root = transform.FindChild("Conveyors");
		_conveyors = root.GetComponentsInChildren<Conveyor>().ToList();
		_conveyors.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));

		foreach (var c in _conveyors)
			c.Speed = ConveyorSpeed;
	}

	private float _speedTimer;

	override protected void Tick()
	{
		UpdateSpeed();

#if !FINAL
		if (Input.GetKeyDown(KeyCode.Return))
		{
			var cake = NewCake();
			cake.transform.position = CakeSpawnPoint.transform.position;
			var truck = FindObjectOfType<Truck>();
			truck.AddCake(cake.GetComponent<Cake>());
		}
#endif

		SpawnCake();
	}

	private void UpdateSpeed()
	{
		_speedTimer -= DeltaTime;
		if (!(_speedTimer < 0)) 
			return;

		_speedTimer = SpeedIncrementTime;
		OverallSpeed *= 1.0f/SpawnIncrementRate;

		foreach (var c in _conveyors)
			c.Speed *= SpeedIncrementRate;

		//Debug.Log("Speed Up " + _speedLevel);

		SpeedLevel++;
	}

	private void SpawnCake()
	{
		_spawnTimer -= DeltaTime;
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
		var conveyor = GetConveyor(height);
		if (!conveyor)
			return null;

		if (right && !conveyor.MoveRight)
			return null;

		return conveyor;
	}

	public Conveyor GetConveyor(int height)
	{
		return height >= _conveyors.Count || height < 0 ? null : _conveyors[height];
	}

	public void Pause(bool pause)
	{
		Paused = pause;

		foreach (var conv in _conveyors)
			conv.Pause(pause);
	}
}
