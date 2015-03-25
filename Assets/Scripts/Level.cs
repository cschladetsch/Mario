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
	/// <summary>
	/// What needs to be delivered on the level
	/// </summary>
	public Dictionary<IngredientType, int> Inventory;

	/// <summary>
	/// The panel that holds what is coming down the pipe
	/// </summary>
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

	/// <summary>
	/// The conveyors in the level. Each level can have different number
	/// of levels.
	/// </summary>
	public IList<Conveyor> Conveyors
	{
		get { return _conveyors; }
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

	/// <summary>
	/// How many actual items needs to be delivered
	/// </summary>
	public int NumCakesRemaining
	{
		get { return Inventory.Sum(c => c.Value); }
	}

	public bool NothingToSpawn
	{
		get { return SpawnsRemaining == 0; }
	}

	public int SpawnsRemaining
	{
		get { return _spawners.Sum(s => s.SpawnsLeft); }
	}

	/// <summary>
	/// How to make new cakes. TODO: make this a dictionary of type to spawner?
	/// </summary>
	public List<SpawnInfo> _spawners = new List<SpawnInfo>();

	private List<Conveyor> _conveyors = new List<Conveyor>();

	private Transform _cakesHolder;

	private Character[] _characters;

	private float _initialConveyorSpeed;

	public void Init()
	{
		_initialConveyorSpeed = ConveyorSpeed;
		_cakesHolder = transform.FindChild("Contents");
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
		Debug.Log("CurrentLevel.BeginLevel");

		_characters = FindObjectsOfType<Character>();
		PauseCharacters(true);

		// In case we BeginLevel twice, remove previous delegate
		Player.OnCakeDropped -= CakeDropped;
		Player.OnCakeDropped += CakeDropped;

		Init();

		Truck.DeliveryCompleted -= DeliveryCompleted;
		Truck.DeliveryCompleted += DeliveryCompleted;

		Reset();

		Player.Reset();

		PauseCharacters(false);

		GatherSpawners();

		if (IncomingPanel == null)
			IncomingPanel = FindObjectOfType<IncomingPanel>();
	}

	private void DeliveryCompleted(Truck truck)
	{
	}

	public void EndLevel()
	{
		Debug.Log("EndLevel " + name);

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
		Inventory[type]--;

		var born = spawnInfo.Spawn();
		//born.transform.position = CakeSpawnPoint.transform.position;
		born.transform.position = pos;
		born.name = Guid.NewGuid().ToString();

		//Debug.Log("Spawned a " + spawnInfo.Prefab.name + " called " + born.name);
		var cake = born.GetComponent<Cake>();

		// if we're in another area, we still want
		// to spawn a cake, but we don't want to show it
		if (cake && !World.CurrentArea.Visual)
			AreaBase.SetVisual(cake.gameObject, false);

		// FRI
		if (ToTruck)
			Truck.AddCake(cake);
	}

	public bool ToTruck;

	public void Reset()
	{
		//Debug.Log("CurrentLevel.Reset");
		foreach (var c in FindObjectsOfType<Cake>())
			Destroy(c.gameObject);

		GatherConveyors();

		if (_initialConveyorSpeed > 0)
			ConveyorSpeed = _initialConveyorSpeed;
		else
			_initialConveyorSpeed = ConveyorSpeed;

		foreach (var c in _conveyors)
			c.Pause(false);
	}

	private void GatherConveyors()
	{
		_conveyors.Clear();

		var root = transform.FindChild("Conveyors");
		_conveyors = root.GetComponentsInChildren<Conveyor>().ToList();
		_conveyors.Sort((a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));

		foreach (var c in _conveyors)
		{
			c.Reset();
			c.Speed = ConveyorSpeed;
		}
	}

	private float _speedTimer;

	protected override void Tick()
	{
		if (!World.Areas[AreaType.Factory].Visual)
			return;

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

	public SpawnInfo GetSpawner(IngredientType type)
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

		SpeedLevel++;
		//Debug.Log("Speed Up " + SpeedLevel);
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
		// Debug.Log("CurrentLevel.Pause: " + pause + " " + _conveyors.Count);

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

		//Debug.Log("Using " + sp.Prefab.name + " prefab to make " + type);
		_spawners.Add(sp);
	}

	public Conveyor GetTopConveyor()
	{
		return _conveyors[_conveyors.Count - 1];
	}

	public void AddIngredients(Dictionary<IngredientType, int> contents)
	{
		//Debug.Log("Level.AddIngredients");

		if (contents.Sum(c => c.Value) == 0)
		{
			Debug.LogWarning("Nothing delivered!");
			World.ChangeArea(AreaType.Bakery);
		}

		// because this is called before level has been created due to
		// SpawnGameObject issues that take many updates to fully expand out
		// to target objects
		if (Inventory == null)
		{
			//Debug.LogWarning("Unexpected - Inventory is null");
			Inventory = IngredientItem.CreateIngredientDict<int>();
		}

		if (IncomingPanel == null)
		{
			//Debug.LogWarning("Unexpected - IncomingPanel is null");
			IncomingPanel = FindObjectOfType<IncomingPanel>();
		}

		foreach (var kv in contents.Where(kv => kv.Value > 0))
		{
			//Debug.Log("Adding a " + kv.Key + " to factory, kv.Value "+kv.Value);
			IncomingPanel.AddItems(kv.Key, kv.Value);
			Inventory[kv.Key] += kv.Value;
		}

		Inventory[IngredientType.Bomb] = 2;
		Inventory[IngredientType.ExtraLife] = 1;

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
		}

		Destroy(cake.gameObject);
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