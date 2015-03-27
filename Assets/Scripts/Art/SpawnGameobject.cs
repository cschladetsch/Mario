using UnityEngine;
using System.Collections;

/// <summary>
/// Create a new instance from a prefab, then optionally delete self
/// </summary>
public class SpawnGameobject : MonoBehaviour
{
	/// <summary>
	/// If true, the spawned object will be a child of this object.
	/// If false, the spawned object will replace this object
	/// </summary>
	public bool spawnNested = false;

	/// <summary>
	/// The prefab to use to make new instance
	/// </summary>
	public GameObject _srcPrefab;

	/// <summary>
	/// What was spawned
	/// </summary>
	internal GameObject Instance;

	private void Awake()
	{
	}

	private void Start()
	{
		Spawn();
	}

	// Spawn new object on and replace locater
	public void Spawn()
	{
		if (_srcPrefab == null)
		{
			//Debug.LogWarning("No Spawn Prefab in Spawner " + name);
			return;
		}

		if (Instance != null)
			return;

		Instance = Instantiate(_srcPrefab) as GameObject;
		if (Instance == null)
		{
			Debug.LogWarning("Failed to spawn prefab from " + _srcPrefab.name);
			return;
		}

		//Debug.Log("Spawning a " + _srcPrefab);

		Instance.name = _srcPrefab.name.ToString();

		// Parent under spawner's parent, or under spawner if nested
		Instance.transform.parent = spawnNested ? transform : transform.parent;

		// Match transform to locator
		Instance.transform.localPosition = transform.localPosition;

		if (spawnNested)
			return;

		// Remove old locator if not nested. But do this later, so
		// that we can be asked what we spawned before we are destroyed.
		Destroy(gameObject, 0.1f);
	}
}