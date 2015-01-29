using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : HasWorld
{
	public GameObject CakePrefab;

	public GameObject CakeSpawnPoint;

	public float MinSpawnTime = 3;

	public float MaxSpawnTime = 6;

	public float OverallSpeed = 1;

	List<Conveyor> _conveyors = new List<Conveyor>();

	private float _spawnTimer;

	private Transform _cakes;

	public void BeginLevel()
	{
		//Debug.Log("Level begins");

		_cakes = transform.FindChild("Cakes");

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
		if (_paused)
			return;

		SpawnCake();
	}

	private void SpawnCake()
	{
		_spawnTimer -= Time.deltaTime;
		if (_spawnTimer > 0)
			return;

		_spawnTimer = OverallSpeed*UnityEngine.Random.Range(MinSpawnTime, MaxSpawnTime);

		var cake = NewCake();
		cake.transform.position = CakeSpawnPoint.transform.position;
	}

	private GameObject NewCake()
	{
		var cake = (GameObject)Instantiate(CakePrefab);
		cake.transform.parent = _cakes;
		return cake;
	}

	public Conveyor GetConveyor(int height, bool right)
	{
		var c = GetConveyor(height);
		if (!c)
			return null;

		if (right && !c.MoveRight)
			return null;

		return c;
	}

	public Conveyor GetConveyor(int height)
	{
		return height >= _conveyors.Count ? null : _conveyors[height];
	}

	private bool _paused;

	public void Pause(bool pause)
	{
		_paused = pause;
		foreach (var conv in _conveyors)
			conv.Pause(pause);
	}
}
