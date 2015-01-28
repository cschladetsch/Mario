using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : HasWorld
{
	public GameObject CakePrefab;

	public float MinSpawnTime = 3;

	public float MaxSpawnTime = 6;

	public float OverallSpeed = 1;

	List<Conveyor> _conveyors = new List<Conveyor>();

	private float _spawnTimer;

	public void BeginLevel()
	{
		Debug.Log("Level begins");

		Reset();

		Player.Reset();
	}

	private void Reset()
	{
		GatherConveyors();
	}

	private void GatherConveyors()
	{
		_conveyors.Clear();

		var root = transform.FindChild("Conveyors");
		_conveyors = root.GetComponentsInChildren<Conveyor>().ToList();
		_conveyors.Sort((a, b) => System.String.Compare(a.name, b.name, System.StringComparison.Ordinal));
	}

	void Update()
	{
		SpawnCake();
	}

	private void SpawnCake()
	{
		_spawnTimer -= Time.deltaTime;
		if (_spawnTimer > 0)
			return;

		_spawnTimer = OverallSpeed*UnityEngine.Random.Range(MinSpawnTime, MaxSpawnTime);

		var cake = (GameObject) Instantiate(CakePrefab);

		// TODO: add from first conveyor when everything is layed out properly
		_conveyors[1].AddCake(cake.GetComponent<Cake>(), 0);
	}

	public Conveyor GetConveyor(int height, bool right)
	{
		var c = GetConveyor(height);
		if (right && !c.MoveRight)
			return null;

		return c;
	}

	public Conveyor GetConveyor(int height)
	{
		if (height >= _conveyors.Count)
			return null;

		return _conveyors[height];
	}
}
