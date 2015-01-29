using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Truck : MonoBehaviour
{
	/// <summary>
	/// How fast to move when emptying
	/// </summary>
	public float MoveSpeed = 1;

	/// <summary>
	/// How far to move when emptying
	/// </summary>
	public float MoveDistance = 4;

	/// <summary>
	/// How long cakes take to fly into the truck
	/// </summary>
	public float FlightTime = 2;

	/// <summary>
	/// Number of columns for the stacked cakes
	/// </summary>
	public int NumColumns = 2;

	/// <summary>
	/// Number of rows for the stacked cakes
	/// </summary>
	public int NumRows = 3;

	private Transform _pt;
	
	private readonly List<Cake> _cakes = new List<Cake>();

	private readonly List<Cake> _pending = new List<Cake>(); 

	private float _moveTime;
	private bool _movingLeft;
	private float _startPos;
	private bool _emptying;
	private bool _full;
	private World _world;

	void Awake()
	{
		_pt = transform.FindChild("FlythroughPoint");
	}

	void Start()
	{
		_startPos = transform.position.x;
		_world = FindObjectOfType<World>();
	}

	void Update()
	{
		UpdateEmptying();

		if (_emptying)
			return;

		MoveCakes();

		UpdateDone();
	}

	private void UpdateEmptying()
	{
		if (!_emptying)
			return;

		_moveTime -= Time.deltaTime;
		var p = transform.position;
		if (_movingLeft)
		{
			transform.position = new Vector3(p.x - MoveSpeed*Time.deltaTime, p.y, 0);
			if (_moveTime <= 0)
			{
				EmptyCakes();

				// reset for the return trip
				_moveTime = MoveDistance/MoveSpeed;
			}
			return;
		}

		transform.position = new Vector3(p.x + MoveSpeed*Time.deltaTime, p.y, 0);
		if (_moveTime <= 0)
		{
			EndEmptying();
		}
	}

	private void EmptyCakes()
	{
		foreach (var cake in _cakes)
			Destroy(cake.gameObject);

		_cakes.Clear();
		_movingLeft = false;
	}

	private void EndEmptying()
	{
		transform.position = new Vector3(_startPos, transform.position.y, 0);

		_emptying = false;
		_full = false;

		foreach (var c in _pending.ToList())
		{
			AddCake(c);
			if (_full)
				break;
			_pending.Remove(c);
		}

		// if there was more than a truck-load pending, just delete the remainder.
		// this probably will never happen in a real game, but can happen in debugging modes
		foreach (var c in _pending)
		{
			Destroy(c.gameObject);
		}

		_pending.Clear();

		_world.Pause(false);
	}

	private void UpdateDone()
	{
		var done = _cakes.Count == NumColumns*NumRows && _cakes.All(c => c.TruckParabola == null);
		if (!done) 
			return;

		_world.Pause(true);

		FindObjectOfType<MarioCamera>().StartTruckAnimation(this);

		_moveTime = MoveDistance/MoveSpeed;
		_movingLeft = true;
		_emptying = true;
	}

	private void MoveCakes()
	{
		foreach (var cake in _cakes)
		{
			var p = cake.TruckParabola;
			if (p == null)	// already landed in truck
				continue;

			var pt = p.Calc(cake.transform.position.x);
			cake.transform.position = new Vector3(pt.x, pt.y, 0);

			var arrived = cake.transform.position.x <= p.FinalPos.x;
			if (arrived)
			{
				cake.transform.position = p.FinalPos;
				cake.transform.parent = transform;
				cake.TruckParabola = null;
			}
		}
	}

	public void AddCake(Cake cake)
	{
		if (!cake)
		{
			Debug.LogError("Trying to add a deleted cake to Truck!");
			return;
		}

		cake.transform.rotation = Quaternion.identity;

		if (_full)
		{
			_pending.Add(cake);
			return;
		}

		int row = _cakes.Count/NumColumns;
		int col = _cakes.Count%NumColumns;
		var width = cake.Width;
		var height = cake.Height;

		var finalPos = transform.position + new Vector3(-1, 1, 0);

		finalPos.x += col*width;
		finalPos.y += row*height;

		float delta = cake.transform.position.x - finalPos.x;
		float dx = -delta/FlightTime;
		cake.TruckParabola = new Parabola(cake.transform.position, _pt.position, finalPos, dx);

		cake.Position = 0;
		_cakes.Add(cake);
		if (_cakes.Count == NumColumns*NumRows)
		{
			_full = true;
		}
	}
}
