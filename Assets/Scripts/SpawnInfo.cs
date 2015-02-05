using UnityEngine;

public class SpawnInfo : MarioObject
{
	public GameObject Prefab;

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

	private Transform _folder;		// where to place newly spawned objects and their splines, if any
	private float _spawnTimer;		// when this reaches zero, this spawned is able to spawn

	protected override void Begin()
	{
		_folder = transform.FindChild("Spawned");

		CalcNextSpawnTime();
	}

	public bool CouldSpawnFromHeight()
	{
		return true;
	}

	public bool CouldSpawn()
	{
		return _spawnTimer < 0 && CouldSpawnFromHeight();
	}

	protected override void Tick()
	{
		_spawnTimer -= DeltaTime;
	}

	public GameObject Spawn(GameObject parent)
	{
		if (Prefab == null)
		{
			Debug.LogError("Spawner " + name + " has no Prefab");
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
}
