using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

	private Character[] _characters;

	private float _initialConveyorSpeed;

	public IList<Conveyor> Conveyors
	{
		get { return _conveyors; }
	}

	public bool NoMoreCakes
	{
		get
		{
			if (Inventory == null)
				return true;

			return Inventory.Sum(c => c.Value) == 0;
		}
	}

	
	public void Init()
	{
		_initialConveyorSpeed = ConveyorSpeed;
		_cakesHolder = transform.FindChild("Contents");

		_size = Camera.main.orthographicSize;
		_height = Camera.main.transform.position.y;

		//Debug.Log("Setting camera");
		Camera.main.orthographicSize = 7.6f;
		Camera.main.transform.SetY(4.8f);
	}

	private float _size;
	private float _height;

	public override void End()
	{
		base.End();
	}

	private void CakeDropped(Player player)
	{
		// spawn another cake
		--_numCakesSpawned;
		//Debug.Log("Cake Dropped: " + _numCakesSpawned + ", " + UnityEngine.Time.frameCount);
	}

	public void BeginLevel()
	{
		//Debug.Log("CurrentLevel.BeginLevel");

		_characters = FindObjectsOfType<Character>();
		PauseCharacters(true);

		Player.OnCakeDropped -= CakeDropped;
		Player.OnCakeDropped += CakeDropped;

		Init();

		Truck.DeliveryCompleted -= DeliveryCompleted;
		Truck.DeliveryCompleted += DeliveryCompleted;

		Reset();

		Player.Reset();

		PauseCharacters(false);

		GatherSpawners();

		// HACKS
		//var cam = Camera.main.transform;
		//cam.position = new Vector3(-1.37f, 4.79f, -10);
		//Camera.main.orthographicSize = 7.62f;
	}

	//private int _numTrucksDelivered;

	private void DeliveryCompleted(Truck truck)
	{
		//Debug.Log("DeliveryCompleted: " + NoMoreCakes);

		if (NoMoreCakes)
			World.ChangeArea(AreaType.Bakery);
	}

	private bool _ended;

	public void EndLevel()
	{
		//Debug.Log("EndLevel " + name);
		_ended = true;

		Camera.main.orthographicSize = _size;
		Camera.main.transform.SetY(_height);
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
		//AddCake(_spawners[0]);
	}

	/// <summary>
	/// Add a new cake to the conveyors
	/// </summary>
	/// <param name="spawnInfo"></param>
	private void AddCake(SpawnInfo spawnInfo)
	{
		if (spawnInfo.Prefab == null)
			return;

		//Debug.Log("AddCake: prefab=" + spawnInfo.Prefab.name + "@" + UnityEngine.Time.frameCount);
		if (!spawnInfo.CanSpawn())
			return;

		var num = Inventory[spawnInfo.Type];
		if (num == 0)
			return;

		Inventory[spawnInfo.Type]--;

		//Debug.Log("Cakes Left: " + Inventory.Sum(c => c.Value));

		var born = spawnInfo.Spawn(gameObject);
		born.transform.position = CakeSpawnPoint.transform.position;
		born.name = Guid.NewGuid().ToString();
		//Debug.Log("Spawned a " + spawnInfo.Prefab.name + " called " + born.name);

		var cake = born.GetComponent<Cake>();
		if (cake != null)
		{
			//if (!Area.Visual)
				//Debug.Log("Spawned a " + cake.name);

			++_numCakesSpawned;
		}

		if (Area == null)
		{
			Debug.LogError("Level as no Area??");
		}

		// if we're in another area, we still want
		// to spawn a cake, but we don't want to show it
		if (cake && !Area.Visual)
			AreaBase.ToggleVisuals(cake.gameObject, false);

		// FRI
		if (ToTruck)
			Truck.AddCake(cake);
	}

	public bool ToTruck;

	private int _numCakesSpawned;

	public void Reset()
	{
		//Debug.Log("CurrentLevel.Reset");

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
		//_numTrucksDelivered = 0;
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

		//Debug.Log("GatherConveyors: " + _conveyors.Count);
	}

	private float _speedTimer;

	public Dictionary<IngredientType, int> Inventory;

	public FactoryArea Area;

	override protected void Tick()
	{
		if (_ended)
			return;

		UpdateSpeed();

#if !FINAL
		if (Input.GetKeyDown(KeyCode.Return))
		{
			//var truck = FindObjectOfType<Truck>();
			//while (Inventory.Count > 0)
			//{
			//	var c = NewCake();
			//	truck.AddCake(c.GetComponent<Cake>());
			//}
			//var cake = NewCake();
			//cake.transform.position = CakeSpawnPoint.transform.position;
			//var truck = FindObjectOfType<Truck>();
			//truck.AddCake(cake.GetComponent<Cake>());
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

	//public int _numCakesSpawned;

	private void UpdateSpeed()
	{
		_speedTimer -= GameDeltaTime;
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

		//// don't spawn anything while truck is emptying
		//if (Truck.Emptying)
		//	return;

		if (_numCakesSpawned == NumTruckLoads*6)
			return;

		var options = _spawners.Where(sp => sp.CouldSpawn()).ToList();
		if (options.Count == 0)
		{
			return;
		}

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
		//Debug.Log("CurrentLevel.Pause: " + pause + " " + _conveyors.Count);

		Paused = pause;

		foreach (var conv in _conveyors)
			conv.Pause(pause);
	}
	/// <summary>
	/// Add spawners for contents of truck. TODO: move to Level.cs
	/// </summary>
	public void AddSpawners(Dictionary<IngredientType, int> contents)
	{
		// create spawners from what was in truck
		var types = new List<IngredientType>();

		// TODO
		if (contents == null)
			return;

		Inventory = contents;

		foreach (var c in Inventory)
		{
			if (c.Value == 0)
				continue;

			var type = c.Key;

			if (types.Contains(type))
				continue;

			types.Add(type);

			var sp = CurrentLevel.gameObject.AddComponent<SpawnInfo>();
			var info = World.IngredientInfo[type];
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

	}

	public Conveyor GetTopConveyor()
	{
		return _conveyors[_conveyors.Count - 1];
	}
}
