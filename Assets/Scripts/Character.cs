using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum WhichSide
	{
		Left,
		Right
	};

	public WhichSide Side;

	public int Height;

	public float MovementSpeed;

	private Vector3 _moveVel;

	public float LevelHeight = 1.5f;

	private Level _level;

	private readonly Transform[] _levels = new Transform[3];

	void Awake()
	{
		_level = FindObjectOfType<Level>();

		_levels[0] = transform.FindChild("Level0");
		_levels[1] = transform.FindChild("Level1");
		_levels[2] = transform.FindChild("Level2");

		var world = FindObjectOfType<World>();

		for (var n = 0; n < 3; ++n)
			_levels[n].parent = world.transform;
	}

	void Start()
	{
	}

	void Update()
	{
		MouseInput();

		TouchInput();

		Move();

		PickupCakes();
	}

	private void TouchInput()
	{
		if (Input.touchCount == 0)
			return;

		ProcessTouch(Input.touches[0].position);
	}

	private void PickupCakes()
	{
		// 0 -> 0,1
		// 1 -> 1,2
		// 2 -> 3,4
		switch (Height)
		{
			case 0:
				PickupFromConveyors(0);
				PickupFromConveyors(1);
				PickupFromConveyors(2);
				break;
			case 1:
				PickupFromConveyors(2);
				PickupFromConveyors(3);
				break;
			case 2:
				PickupFromConveyors(4);
				PickupFromConveyors(5);
				break;
		}


	}

	private void PickupFromConveyors(int level)
	{
		var conv = _level.GetConveyor(level);
		var next = _level.GetConveyor(level + 1);

		if (!conv)
			return;

		foreach (var cake in conv.Cakes.ToList())
		{
			if (!cake.Hanging)
				continue;

			var dist = Mathf.Abs(transform.position.x - cake.transform.position.x);
			if (dist > 4)
				continue;

			conv.RemoveCake(cake);
			if (next)
			{
				next.AddCake(cake, 0);
			}
		}
	}

	private void Move()
	{
		var target = _levels[Height].position;
		transform.position = Vector3.SmoothDamp(transform.position, target, ref _moveVel, MovementSpeed);
	}

	private void MouseInput()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		ProcessTouch(Input.mousePosition);
	}

	private void ProcessTouch(Vector3 pos)
	{
		if (Side == WhichSide.Left && pos.x > Screen.width*0.4f)
			return;

		if (Side == WhichSide.Right && pos.x < Screen.width*0.6f)
			return;

		var screen = Camera.main.WorldToScreenPoint(transform.position);
		if (pos.y < screen.y)
			MoveDown();
		else
			MoveUp();
	}

	private void MoveUp()
	{
		if (Height == 2)
			return;

		Height++;
	}

	private void MoveDown()
	{
		if (Height == 0)
			return;

		Height--;
	}

	public void Reset()
	{
		Height = 0;
	}
}


