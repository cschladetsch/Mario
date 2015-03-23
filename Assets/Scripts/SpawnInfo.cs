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

	private Transform _folder; // where to place newly spawned objects and their splines, if any

	public float _spawnTimer; // when this reaches zero, this spawned is able to spawn

	public int SpawnsLeft;

	protected override void Begin()
	{
		_folder = transform.FindChild("Spawned");

		CalcNextSpawnTime();

		SpawnsLeft = MaxSpawns;
	}

	public bool CouldSpawn()
	{
		return _spawnTimer < 0 && SpawnsLeft > 0;
	}

	protected override void Tick()
	{
		if (SpawnsLeft == 0)
			return;

		_spawnTimer -= GameDeltaTime;
	}

	public GameObject Spawn()
	{
		if (SpawnsLeft-- <= 0)
		{
			CalcNextSpawnTime();
			return null;
		}

		if (Prefab == null)
		{
			Debug.LogError("Spawner has no Prefab");
			return null;
		}

		CalcNextSpawnTime();

		var born = (GameObject) Instantiate(Prefab);
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
		return SpawnsLeft > 0;
	}

	public void SpawnMore(int num)
	{
		SpawnsLeft += num;
		MaxSpawns += num;
	}
}