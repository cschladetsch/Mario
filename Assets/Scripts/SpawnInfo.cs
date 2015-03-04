using UnityEngine;

/// <summary>
/// Used to create new object periodically
/// </summary>
public class SpawnInfo : MarioObject
{
	/// <summary>
	/// How to make the object
	/// </summary>
	public GameObject Prefab;

	public IngredientType Type;

	/// <summary>
	/// The relative weight of spawning this pickup
	/// </summary>
	public int Weight = 1;

	/// <summary>
	/// The shortest time between two spawns
	/// </summary>
	public float MinSpawnTime = 10;

	/// <summary>
	/// The longest time between two spawns
	/// </summary>
	public float MaxSpawnTime = 20;

	/// <summary>
	/// if -1, then always spawn
	/// </summary>
	public int MaxSpawns = -1;

	private Transform _folder;		// where to place newly spawned objects and their splines, if any
	private float _spawnTimer;		// when this reaches zero, this spawned is able to spawn
	private int _spawnsLeft;

	protected override void Begin()
	{
		_folder = transform.FindChild("Spawned");

		CalcNextSpawnTime();

		_spawnsLeft = MaxSpawns;
	}

	public bool CouldSpawnFromHeight()
	{
		return true;
	}

	public bool CouldSpawn()
	{
		return _spawnTimer < 0 && CouldSpawnFromHeight() && _spawnsLeft > 0;
	}

	protected override void Tick()
	{
		_spawnTimer -= DeltaTime;
	}

	public GameObject Spawn(GameObject parent)
	{
		if (_spawnsLeft-- <= 0)
			return null;

		if (Prefab == null)
		{
			Debug.LogError("Spawner has no Prefab");
			return null;
		}

		CalcNextSpawnTime();

		var born = (GameObject)Instantiate(Prefab);
		if (born == null)
		{
			Debug.LogError("Unable to spawn " + Prefab.name);
			return null;
		}

		born.GetComponent<Pickup>().Create(_folder);
		born.transform.parent = _folder;
		return born;
	}

	private float CalcNextSpawnTime()
	{
		return _spawnTimer = Random.Range(MinSpawnTime, MaxSpawnTime);
	}

	public bool CanSpawn()
	{
		return _spawnsLeft > 0;
	}
}
