using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
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
	readonly List<Cake> _cakes = new List<Cake>();

	/// <summary>
	/// Cached collision box
	/// </summary>
	private BoxCollider2D _box;

	private bool _paused;

	void Awake()
	{
		_box = GetComponentInChildren<BoxCollider2D>();
	}

	void Start()
	{
	}

	/// <summary>
	/// Add a cake to the conveyor
	/// </summary>
	/// <param name="cake">the cake to add</param>
	/// <param name="pos">where to add it, normalised to the length of the conveyor</param>
	public void AddCake(Cake cake, float pos)
	{
		cake.Reset();
		cake.Position = pos;
		_cakes.Add(cake);
	}

	void Update()
	{
		if (_paused)
			return;

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
		foreach (var cake in _cakes)
			MoveCake(cake);
	}

	private void MoveCake(Cake cake)
	{
		if (cake.Hanging)
			return;

		cake.Position += Speed*Time.deltaTime;
		if (cake.Position > 1)
		{
			cake.StartHanging();
			return;
		}

		var dist = cake.Position*_box.bounds.size.x;
		var loc = _box.bounds.min.x + dist;
		if (!MoveRight)
			loc = _box.bounds.max.x - dist;

		cake.gameObject.transform.position = new Vector3(loc, transform.position.y + 1, 0);
	}

	public void RemoveCake(Cake cake)
	{
		_cakes.Remove(cake);
	}

	public void Pause(bool pause)
	{
		_paused = pause;
	}
}
