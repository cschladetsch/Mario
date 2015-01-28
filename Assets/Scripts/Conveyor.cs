using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
	public float Speed = 1;

	public bool MoveRight;

	readonly List<Cake> _cakes = new List<Cake>();

	private BoxCollider2D _box;

	public IList<Cake> Cakes { get { return _cakes; }}

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
		MoveCakes();

		foreach (var cake in _cakes)
			cake.UpdateCake(MoveRight);
	}

	private void MoveCakes()
	{
		foreach (var cake in _cakes.ToList())
			MoveCake(cake);
	}

	private void MoveCake(Cake cake)
	{
		if (cake.Dropped)
		{
			_cakes.Remove(cake);
			return;
		}

		if (cake.Hanging)
		{
			return;
		}

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
}


