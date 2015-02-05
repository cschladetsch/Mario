using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls aspects of the level, like the conveyors, characters, and cakes
/// </summary>
public class Level : MarioObject
{
	public int NumTruckLoads = 3;

	/// <summary>
	/// Where to place newly made cake
	/// </summary>
	public GameObject CakeSpawnPoint;

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

	/// <summary>
	/// How to make new cakes
	/// </summary>
	private SpawnInfo[] _spawners;

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

		Debug.Log("Level created " + name);
	}

	public void BeginLevel()
	{
		Debug.Log("Level begins");

		Truck.DeliveryCompleted -= DeliveryCompleted;
		Truck.DeliveryCompleted += DeliveryCompleted;

		Reset();

		Player.Reset();

		PauseCharacters(false);

		GatherSpawners();
	}

	private int _numDelivered;

	private void DeliveryCompleted(Truck truck)
	{
		if (++_numDelivered == NumTruckLoads)
		{
			EndLevel();
		}
	}

	private void EndLevel()
	{
		
	}

	private void GatherSpawners()
	{
		_spawners = GetComponents<SpawnInfo>();
	}

	private void PauseCharacters(bool pause)
	{
		foreach (var ch in _characters)
			ch.Pause(pause);
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		SpawnSomething();
	}

	private void SpawnSomething()
	{
		_spawners[0].Spawn(gameObject);
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
		OverallSpeed = 1;

		_speedTimer = SpeedIncrementTime;
		_numDelivered = 0;
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
		UpdateSpawners();

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

	private void UpdateSpawners()
	{
		var options = _spawners.Where(sp => sp.CouldSpawn()).ToList();
		if (options.Count == 0)
			return;

		var spawner = SelectRandomWeighted(options);
		var born  = spawner.Spawn(gameObject);
		born.transform.position = CakeSpawnPoint.transform.position;
	}

	/// <summary>
	/// Select an option, using weighted random selection system
	/// </summary>
	/// <param name="options"></param>
	/// <returns></returns>
	private static SpawnInfo SelectRandomWeighted(List<SpawnInfo> options)
	{
		var sumOfWeight = options.Sum(t => t.Weight);
		var rnd = UnityEngine.Random.Range(0, sumOfWeight);
		options.Sort((a, b) => a.Weight.CompareTo(b.Weight));

		var chosen = 0;
		for (var n = 0; n < options.Count; n++)
		{
			if (rnd < options[n].Weight)
			{
				chosen = n;
				break;
			}

			rnd -= options[n].Weight;
		}

		return options[chosen];
	}

	private GameObject NewCake()
	{
		var prefab = GetNewPrefab();
		var cake = (GameObject)Instantiate(prefab);
		cake.transform.parent = _cakesHolder;
		return cake;
	}

	//void LateUpdate()
	//{
	//	transform.position = Vector3.zero;
	//}

	private GameObject GetNewPrefab()
	{
		return _spawners[0].Prefab;
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
