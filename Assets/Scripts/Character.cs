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

	void Awake()
	{
		_level = FindObjectOfType<Level>();
	}

	void Start()
	{
	}

	void Update()
	{
		MouseInput();

		Move();

		PickupCakes();
	}

	private void PickupCakes()
	{
		var right = transform.position.x > 0;
		var conv = _level.GetConveyor(Height + 1, right);
		var next = _level.GetConveyor(Height + 2);

		if (!conv)
			return;

		foreach (var cake in conv.Cakes.ToList())
		{
			if (!cake.Hanging)
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
		var pp = transform.position;
		var target = new Vector3(pp.x, Height*LevelHeight, 0);
		transform.position = Vector3.SmoothDamp(transform.position, target, ref _moveVel, MovementSpeed);
	}

	private void MouseInput()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		var pos = Input.mousePosition;
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
		if (Height == 4)
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


