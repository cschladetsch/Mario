using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MarioObject
{
	public enum WhichSide
	{
		Left,
		Right
	};

	public WhichSide Side;

	public int Height;

	public float MovementSpeed;

	public float LevelHeight = 1.5f;

	private Vector3 _moveVel;

	private readonly Transform[] _levels = new Transform[3];

	private bool _touching;

	private StoplightBoss1 _stoplightBoss1;

	protected override void Begin()
	{
		base.Begin();

		_stoplightBoss1 = FindObjectOfType<StoplightBoss1>();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		_levels[0] = transform.FindChild("Level0");
		_levels[1] = transform.FindChild("Level1");
		_levels[2] = transform.FindChild("Level2");

		for (var n = 0; n < 3; ++n)
			_levels[n].parent = World.transform;

		Paused = true;
	}

	public bool Spasming { get { return _spasmTimer > 0; } }

	protected override void Tick()
	{
		base.Tick();

		_spasmTimer -= GameDeltaTime;

#if UNITY_EDITOR
		MouseInput();
#endif

		TouchInput();

		Move();

		PickupCakes();
	}

	private void TouchInput()
	{
		if (Input.touchCount == 0)
		{
			_touching = false;
			return;
		}

		var mid = Screen.width*0.5f;
		var touch = false;
		foreach (var t in Input.touches)
		{
			if (Side == WhichSide.Left)
			{
				if (t.position.x < mid)
				{
					if (!_touching)
					{
						ProcessTouch(t.position);
						_touching = true;
					}
					touch = true;
				}
				continue;
			}

			if (t.position.x > mid)
			{
				if (!_touching)
				{
					ProcessTouch(t.position);
					_touching = true;
				}
				touch = true;
			}
		}

		_touching = touch;
	}

	private void PickupCakes()
	{
		switch (Height)
		{
			case 0:
				PickupFromConveyors(0);
				PickupFromConveyors(1);
				if (Side == WhichSide.Left)
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
		var conv = CurrentLevel.GetConveyor(level);
		var next = CurrentLevel.GetConveyor(level + 1);

		if (!conv)
			return;

		foreach (var item in conv.Contents.ToList())
		{
			if (!item.Hanging)
				continue;

			var dist = Mathf.Abs(transform.position.x - item.transform.position.x);
			if (dist > 4)
				continue;

			item.CharacterHit(this, conv, next);
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

		var ratio = Screen.height/pos.y;
		//Debug.Log(ratio);

		// don't respond to button presses to change area
		if (Side == WhichSide.Right && ratio < 1.44f)
			return;

		if (Side == WhichSide.Right && pos.x < Screen.width*0.6f)
			return;

		var screen = Camera.main.WorldToScreenPoint(transform.position);

		// account for SpotlightBoss: if it is facing us, reverse controls
		var moveDown = pos.y < screen.y;
		if (_stoplightBoss1)
		{
			if (_stoplightBoss1.Dir == StoplightBoss1.Direction.Left && Side == WhichSide.Left)
				moveDown = !moveDown;

			if (_stoplightBoss1.Dir == StoplightBoss1.Direction.Right && Side == WhichSide.Right)
				moveDown = !moveDown;
		}

		if (moveDown)
			MoveDown();
		else
			MoveUp();
	}

	private void MoveUp()
	{
		//Debug.Log("MoveUp " + Time.frameCount);
		if (Height == 2)
			return;

		Height++;
	}

	private void MoveDown()
	{
		//Debug.Log("MoveDown " + Time.frameCount);
		if (Height == 0)
			return;

		Height--;
	}

	public void Reset()
	{
		Height = 0;
	}

	public void Pause(bool pause)
	{
		//Debug.Log("Character.Pause: " + pause);
		Paused = pause;
	}

	private float _spasmTimer;

	public void Spasm(float spasmLength)
	{
		Debug.Log("Spasm " + Side);
		_spasmTimer = spasmLength;
	}
}
