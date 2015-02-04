using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Conveyor : HasWorld
{
	/// <summary>
	/// Move speed of the conveyor
	/// </summary>
	public float Speed = 1;

	/// <summary>
	/// If false, we are moving from left to right
	/// </summary>
	public bool MoveRight;

	/// <summary>
	/// The cakes currently on this conveyor
	/// </summary>
	public IList<Cake> Cakes { get { return _cakes; } }

	/// <summary>
	/// The cakes on this conveyor
	/// </summary>
	public List<Cake> _cakes = new List<Cake>();

	/// <summary>
	/// Cached collision box
	/// </summary>
	private BoxCollider2D _box;

	protected override void Begin()
	{
		_box = GetComponentInChildren<BoxCollider2D>();
	}

	/// <summary>
	/// Add a cake to the conveyor
	/// </summary>
	/// <param name="cake">the cake to add</param>
	/// <param name="pos">where to add it, normalised to the length of the conveyor</param>
	public void AddCake(Cake cake, float pos)
	{
		//Debug.Log("AddCake " + cake.name);
		cake.Reset();
		cake.Position = pos;
		cake.Conveyor = this;
		_cakes.Add(cake);
	}

	protected override void Tick()
	{
		UpdateCakes();

		MoveCakes();
	}

	private void UpdateCakes()
	{
		foreach (var cake in _cakes.ToList())
		{
			cake.UpdateCake(MoveRight);

			if (cake.Dropped)
				RemoveCake(cake);
		}
	}

	private void MoveCakes()
	{
		if (_cakes.Count == 0)
			return;

		// sort by position in x
		_cakes.Sort((a,b) => a.transform.position.x.CompareTo(b.transform.position.x));
		for (int n = 0; n < _cakes.Count - 1; ++n)
		{
			var curr = _cakes[n];
			var next = _cakes[n + 1];

			if (Mathf.Abs(curr.transform.position.x - next.transform.position.x) < 0.01f)
			{
				curr.Position -= 0.05f;
			}
		}

		foreach (var cake in _cakes)
			MoveCake(cake);
	}

	public float MinCakeSeparation = 1;
	
	private bool MoveCake(Cake cake)
	{
		if (cake.Hanging)
		{
			cake.Moved = true;
			return false;
		}

		//Cake closest;
		//var sep = float.MaxValue;
		var mx = cake.transform.position.x;
		var move = true;
		foreach (var c in _cakes)
		{
			if (c == cake)
				continue;

			var cx = c.transform.position.x;

			if (Mathf.Abs(cx - mx) < 0.01f)
				continue;

			if (MoveRight)
			{
				if (cx < mx)
					continue;

				if (cx - mx < MinCakeSeparation)
				{
					move = false;
					break;
				}
			}
			else
			{
				if (cx > mx)
					continue;

				if (mx - cx < MinCakeSeparation)
				{
					move = false;
					break;
				}
			}
		}

		cake.Moved = move;
		if (move)
			cake.Position += Speed*DeltaTime;

		if (cake.Position > 1)
		{
			cake.StartHanging();
			return false;
		}

		var dist = cake.Position*_box.bounds.size.x;
		var loc = _box.bounds.min.x + dist;
		if (!MoveRight)
			loc = _box.bounds.max.x - dist;

		cake.gameObject.transform.position = new Vector3(loc, transform.position.y + 1, 0);

		return true;
	}

	public void RemoveCake(Cake cake)
	{
		_cakes.Remove(cake);
	}

	public void Pause(bool pause)
	{
		Paused = pause;
	}

	public void Reset()
	{
		foreach (var c in _cakes)
			Destroy(c.gameObject);

		_cakes.Clear();
	}
}
