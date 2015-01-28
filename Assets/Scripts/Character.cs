using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public enum WhichSide
	{
		Left,
		Right
	};

	public WhichSide Side;

	public int Level;

	public float MovementSpeed;

	private Vector3 _moveVel;
	public float LevelHeight = 1.5f;

	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
		MouseInput();

		Move();
	}

	private void Move()
	{
		var pp = transform.position;
		var target = new Vector3(pp.x, Level*LevelHeight, 0);
		transform.position = Vector3.SmoothDamp(transform.position, target, ref _moveVel, MovementSpeed);
	}

	private void MouseInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
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
	}

	private void MoveUp()
	{
		if (Level == 6)
			return;

		Level++;
	}

	private void MoveDown()
	{
		if (Level == 0)
			return;

		Level--;
	}

	public void Reset()
	{
		Level = 0;
	}
}


