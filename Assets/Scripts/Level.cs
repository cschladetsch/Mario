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
	public IList<Conveyor> Conveyors { get { return _conveyors; } }

	void Init()
	{
		_initialConveyorSpeed = ConveyorSpeed;
		_characters = FindObjectsOfType<Character>();
		_cakesHolder = transform.FindChild("Contents");

		//Debug.Log("Level.Init: " + ConveyorSpeed);

		PauseCharacters(true);

		Player.OnCakeDropped -= CakeDropped;
		Player.OnCakeDropped += CakeDropped;
	}

	private void CakeDropped(Player player)
	{
		// spawn another cake
		--_numCakesSpawned;
		//Debug.Log("Cake Dropped: " + _numCakesSpawned + ", " + UnityEngine.Time.frameCount);
	}

	public void BeginLevel()
	{
		//Debug.Log("Level.BeginConveyorLevel: " + name);

		Init();

		Truck.DeliveryCompleted -= DeliveryCompleted;
		Truck.DeliveryCompleted += DeliveryCompleted;

		Reset();

		Player.Reset();

		PauseCharacters(false);

		GatherSpawners();
	}

	private int _numTrucksDelivered;

	private void DeliveryCompleted(Truck truck)
	{
		if (++_numTrucksDelivered == NumTruckLoads)
		{
			EndLevel();
		}
	}

	private bool _ended;

	private void EndLevel()
	{
		Debug.Log("EndLevel " + name);
		_ended = true;

		Canvas.LevelEnded(this);

		foreach (var c in _conveyors)
			c.Pause(true);
	}

	private void GatherSpawners()
	{
		_spawners = GetComponents<SpawnInfo>();
	}

	/// <summary>
	/// Pause the characters so they cannot be moved when the game is paused
	/// </summary>
	/// <param name="pause"></param>
	private void PauseCharacters(bool pause)
	{
		foreach (var ch in _characters)
			ch.Pause(pause);
	}

	/// <summary>
	/// Spawn something at the start of the level, so there is no initial pause
	/// </summary>
	private void SpawnSomething()
	{
		AddCake(_spawners[0]);
	}

	/// <summary>
	/// Add a new cake to the conveyors
	/// </summary>
	/// <param name="spawnInfo"></param>
	private void AddCake(SpawnInfo spawnInfo)
	{
		if (_numCakesSpawned == NumTruckLoads*6)
			return;

		var born = spawnInfo.Spawn(gameObject);
		born.transform.position = CakeSpawnPoint.transform.position;

		var cake = born.GetComponent<Cake>();
		if (cake != null)
			++_numCakesSpawned;
	}

	public void Reset()
	{
		GatherConveyors();

		if (_initialConveyorSpeed > 0)
			ConveyorSpeed = _initialConveyorSpeed;
		else
			_initialConveyorSpeed = ConveyorSpeed;

		foreach (var c in _conveyors)
		{
			c.Reset();
			c.Speed = _initialConveyorSpeed;
		}

		SpeedLevel = 1;
		OverallSpeed = 1;

		_speedTimer = SpeedIncrementTime;
		_numTrucksDelivered = 0;
		_numCakesSpawned = 0;
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
		if (_ended)
			return;

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

	/// <summary>
	/// Spawn cakes after main update, to account for any dropped cakes
	/// </summary>
	void LateUpdate()
	{
		if (Paused)
			return;

		if (_numCakesSpawned < NumTruckLoads*6)
			UpdateSpawners();
	}

	public int _numCakesSpawned;

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
		if (_spawners == null)
			return;

		if (_numCakesSpawned == NumTruckLoads*6)
			return;

		var options = _spawners.Where(sp => sp.CouldSpawn()).ToList();
		if (options.Count == 0)
			return;

		AddCake(SelectRandomWeighted(options));
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
		++_numCakesSpawned;
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
		//Debug.Log("Level.Pause: " + pause + " " + _conveyors.Count);

		Paused = pause;

		foreach (var conv in _conveyors)
			conv.Pause(pause);
	}

	public Conveyor GetTopConveyor()
	{
		return _conveyors[_conveyors.Count - 1];
	}
}
