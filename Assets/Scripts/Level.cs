using System;
using System.Collections.Generic;
using System.Linq;
using Flow;
using UnityEngine;

/// <summary>
/// Controls aspects of the level, like the conveyors, characters, and cakes
/// </summary>
public class Level : MarioObject
{
	public int NumTruckLoads = 3;

	public IncomingPanel IncomingPanel;

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
	/// How to make new cakes. TODO: make this a dictionary of type to spawner?
	/// </summary>
	public List<SpawnInfo> _spawners = new List<SpawnInfo>();

	private Transform _cakesHolder;

	private Character[] _characters;

	private float _initialConveyorSpeed;

	public IList<Conveyor> Conveyors
	{
		get { return _conveyors; }
	}

	public int NumCakesRemaining
	{
		get { return Inventory.Sum(c => c.Value); }
	}

	public bool NoMoreCakes
	{
		get
		{
			if (Inventory == null)
			{
				//Debug.LogWarning("Level.NoMoreCakes: Null Inventory");
				return true;
			}

			if (Conveyors.Any(c => c.Contents.Count > 0))
				return false;

			return NumCakesRemaining == 0;
		}
	}

	public int SpawnsRemaining
	{
		get { return _spawners.Sum(s => s.SpawnsLeft); }
	}

	public bool NothingToSpawn
	{
		get { return SpawnsRemaining == 0; }
	}

	public void Init()
	{
		_initialConveyorSpeed = ConveyorSpeed;
		_cakesHolder = transform.FindChild("Contents");
	}

	public override void End()
	{
		base.End();
	}

	private void CakeDropped(Player player)
	{
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		Inventory = IngredientItem.CreateIngredientDict<int>();
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

		IncomingPanel = FindObjectOfType<IncomingPanel>();
	}

	//private int _numTrucksDelivered;

	private void DeliveryCompleted(Truck truck)
	{
		//Debug.Log("DeliveryCompleted: " + NoMoreCakes);
	}

	public void EndLevel()
	{
		//Debug.Log("EndLevel " + name);
		//_ended = true;

		Canvas.LevelEnded(this);

		foreach (var c in _conveyors)
			c.Pause(true);
	}

	private void GatherSpawners()
	{
		_spawners.AddRange(GetComponents<SpawnInfo>());
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
		if (spawnInfo == null)
		{
			Debug.LogWarning("Level.AddCake: Null spawnInfo");
			return;
		}

		//Debug.Log("Adding a " + spawnInfo.Type);

		if (spawnInfo.Prefab == null)
		{
			Debug.LogWarning("Level.AddCake: Spawner for " + spawnInfo.Type + " has no prefab");
			return;
		}

		//Debug.Log("AddCake: prefab=" + spawnInfo.Prefab.name + "@" + UnityEngine.Time.frameCount);
		if (!spawnInfo.CanSpawn())
		{
			Debug.Log("Spawner " + spawnInfo.Type + " cannot spawn");
			return;
		}

		var type = spawnInfo.Type;
		if (!Inventory.ContainsKey(type))
		{
			Debug.Log("Can't make a " + type);
			return;
		}

		var num = Inventory[type];
		if (num == 0)
		{
			Debug.Log("Don;t need to make a " + type);
			return;
		}

		// TODO: 
		var pos = IncomingPanel.RemoveCake(type);
		//Debug.Log(pos);
		Inventory[type]--;

		//Debug.Log("Cakes Left: " + Inventory.Sum(c => c.Value));

		var born = spawnInfo.Spawn(gameObject);
		born.transform.position = CakeSpawnPoint.transform.position;
		//born.GetRectTransform().anchoredPosition = pos;
		born.name = Guid.NewGuid().ToString();
		//Debug.Log("Spawned a " + spawnInfo.Prefab.name + " called " + born.name);

		var cake = born.GetComponent<Cake>();

		if (Area == null)
		{
			Debug.LogError("Level as no Area??");
			return;
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

	public void Reset()
	{
		//Debug.Log("CurrentLevel.Reset");

		GatherConveyors();

		if (_initialConveyorSpeed > 0)
			ConveyorSpeed = _initialConveyorSpeed;
		else
			_initialConveyorSpeed = ConveyorSpeed;
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

	public Dictionary<IngredientType, int> Inventory;

	public FactoryArea Area;

	protected override void Tick()
	{
		if (!World.Areas[AreaType.Factory].Visual)
			return;

		////Debug.Log(string.Format("NoMore: {0}, Emptying: {1}", NoMoreCakes, Truck.Emptying));
		//if (NoMoreCakes && !Truck.Emptying)
		//{
		//	_ended = true;
		//	Truck.StartEmptying();
		//	return;
		//}

		UpdateSpeed();

#if !FINAL
		// throw a cake to truck
		if (Input.GetKeyDown(KeyCode.Return))
		{
			foreach (var conveyor in Conveyors)
			{
				if (conveyor.Contents.Count > 0)
				{
					var go = conveyor.Contents[0];
					conveyor.RemoveItem(go);
					var cake = go.GetComponent<Cake>();
					Truck.AddCake(cake);
					NewRandomCake();
					Inventory[cake.Type]--;
					break;
				}
			}
		}
#endif
	}

	private void NewRandomCake()
	{
		foreach (var kv in Inventory.Where(kv => kv.Value > 0))
		{
			AddCake(GetSpawner(kv.Key));
			return;
		}
	}

	private SpawnInfo GetSpawner(IngredientType type)
	{
		return _spawners.FirstOrDefault(sp => sp.Type == type);
	}

	/// <summary>
	/// Spawn cakes after main update, to account for any dropped cakes
	/// </summary>
	private void LateUpdate()
	{
		//Debug.Log("Level.LateUpdate: Paused " + Paused + " NoMoreCakes: "+ NoMoreCakes);
		if (Paused)
			return;

		if (!NoMoreCakes)
			UpdateSpawners();
	}

	private void UpdateSpeed()
	{
		_speedTimer -= GameDeltaTime;
		if (!(_speedTimer < 0))
			return;

		_speedTimer = SpeedIncrementTime;
		OverallSpeed *= 1.0f/SpawnIncrementRate;

		foreach (var c in _conveyors)
			c.Speed *= SpeedIncrementRate;

		Debug.Log("Speed Up " + SpeedLevel);

		SpeedLevel++;
	}

	private void UpdateSpawners()
	{
		if (_spawners == null)
		{
			Debug.Log("UpdateSpawners: No spawners");
			return;
		}

		if (_spawners.Count == 0)
		{
			Debug.Log("UpdateSpawners: No spawners");
			return;
		}

		if (NoMoreCakes)
		{
			Debug.Log("UpdateSpawners: no more cakes");
			return;
		}

		var options = _spawners.Where(sp => sp.CouldSpawn() && Inventory[sp.Type] > 0).ToList();
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
		var cake = (GameObject) Instantiate(prefab);
		cake.transform.parent = _cakesHolder;
		return cake;
	}

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
	/// Add spawners for contents of truck.
	/// </summary>
	public void AddSpawners(Dictionary<IngredientType, int> contents)
	{
		if (contents == null)
			return;

		foreach (var kv in contents)
		{
			var type = kv.Key;
			var amount = kv.Value;

			// no amount to add
			if (amount == 0)
				continue;

			// don't add nothing
			if (type == IngredientType.None)
				continue;

			// see if we already have a spawner
			var curr = _spawners.FirstOrDefault(s => s.Type == type);
			if (curr)
			{
				// make sure we make more of them later
				curr.SpawnMore(kv.Value);
				continue;
			}

			AddSpawner(kv);
		}
	}

	/// <summary>
	/// Add a spawner for a given ingredient type
	/// </summary>
	/// <param name="kv"></param>
	private void AddSpawner(KeyValuePair<IngredientType, int> kv)
	{
		var sp = CurrentLevel.gameObject.AddComponent<SpawnInfo>();
		var type = kv.Key;
		var info = World.IngredientInfo[type];

		sp.MinSpawnTime = info.MinSpawnRate;
		sp.MaxSpawnTime = info.MaxSpawnRate;
		sp.Weight = 1;
		sp.MaxSpawns = kv.Value;
		sp.Type = type;

		// load prefabs to make ingredients from resources path
		var path = string.Format("{0}", type);
		var ob = Resources.Load(path);
		if (ob == null)
		{
			Debug.LogWarning("No prefab for ingredient " + type);
			return;
		}

		sp.Prefab = (GameObject) ob;
		if (sp.Prefab == null)
		{
			Debug.LogWarning("Can't make a " + type + ", using path " + path);
			return;
		}

		//Debug.Log("Using " + sp.Prefab.name + " prefab to make " + c.Key);

		_spawners.Add(sp);
	}

	public Conveyor GetTopConveyor()
	{
		return _conveyors[_conveyors.Count - 1];
	}

	public void AddIngredients(Dictionary<IngredientType, int> contents)
	{
		//Debug.Log("Level.AddIngredients");

		// because this is called before level has been created due to
		// SpawnGameObject issues that take many updates to fully expand out
		// to target objects
		if (Inventory == null)
			Inventory = IngredientItem.CreateIngredientDict<int>();

		if (IncomingPanel == null)
			IncomingPanel = FindObjectOfType<IncomingPanel>();

		foreach (var kv in contents)
		{
			IncomingPanel.AddItems(kv.Key, kv.Value);
			Inventory[kv.Key] += kv.Value;
		}

		AddSpawners(Inventory);
	}

	/// <summary>
	/// This is really a hack-work around. If a cake has been 'dropped'
	/// but hasn't eventually been destroyed, or for any other weird reason,
	/// then this will safely remove the cake from the system.
	/// </summary>
	/// <param name="cake"></param>
	public void DestroyCake(Cake cake)
	{
		// TODO: maybe also remove from truck?

		// remove cake from whatever conveyor it is in
		foreach (var c in _conveyors.Where(c => c.Contents.Contains(cake)))
		{
			c.RemoveItem(cake);
			Destroy(cake);
			return;
		}
	}

	public void ResetSpeed()
	{
		_speedTimer = SpeedIncrementTime;
		SpeedLevel = 0;
	}

	public void TestForEnd()
	{
		if (NoMoreCakes)
		{
			Truck.StartEmptying();
		}
	}
}