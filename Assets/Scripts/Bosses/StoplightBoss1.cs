using System;
using UnityEngine;

/// <summary>
/// The Stoplight Boss logic.
/// 
/// When facing Left, the left character has its controls inverted.
///
/// When facing Right, the right character has its controls inverted.
/// </summary>
public class StoplightBoss1 : Boss
{
	public enum Direction
	{
		Left, Right, Forward, Transitioning
	}

	/// <summary>
	/// The direction the boss is facing
	/// </summary>
	public Direction Dir;

	/// <summary>
	/// Max rotation in either direction
	/// </summary>
	public float Rotation = 75;

	public float WaitTime = 5;

	public float TurnSpeed;

	private Transform _visual;

	private float _angle;

	public Direction _targetDir;

	public float TurnTimer;

	public float _waitTimer;

	protected override void Begin()
	{
		_visual = transform.FindChild("Pivot");

		Reset();
	}

	public void Reset()
	{
		base.Begin();

		Dir = Direction.Forward;

		_angle = 0;

		_waitTimer = WaitTime;
	}

	protected override void Tick()
	{
		base.Tick();

		// move between orientations
		UpdateRotation();

		_waitTimer -= RealDeltaTime;
		if (_waitTimer < 0)
		{
			_waitTimer = WaitTime;
			RandomTurn();
		}

	}

	/// <summary>
	/// Update the transition from one orientation to the next
	/// </summary>
	private void UpdateRotation()
	{
		// no longer transitioning to new orientation
		if (Dir != Direction.Transitioning)
			return;

		var dir = 1;
		switch (_targetDir)
		{
			case Direction.Forward:
				if (_angle < 0)
					dir = 1;
				else
					dir = -1;
				break;

			case Direction.Left:
				dir = 1;
				break;

			case Direction.Right:
				dir = -1;
				break;
		}

		_angle += dir*TurnSpeed*RealDeltaTime;
		//Debug.Log(_angle);

		switch (_targetDir)
		{
			case Direction.Forward:
				if (Mathf.Abs(_angle) < 1)
				{
					transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
					Dir = Direction.Forward;
					_waitTimer = WaitTime;
				}
				break;

			case Direction.Right:
				if (_angle < -Rotation)
				{
					transform.rotation = Quaternion.AngleAxis(Rotation, Vector3.up);
					Dir = Direction.Right;
					_waitTimer = WaitTime;
				}
				break;

			case Direction.Left:
				if (_angle > Rotation)
				{
					transform.rotation = Quaternion.AngleAxis(-Rotation, Vector3.up);
					Dir = Direction.Left;
					_waitTimer = WaitTime;
				}
				break;
		}

		_visual.transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
	}

	private void RandomTurn()
	{
		var values = Enum.GetValues(typeof(Direction));

		// ensure we transition to a new direction
		while (true)
		{
			_targetDir = (Direction)values.GetValue(UnityEngine.Random.Range(0, values.Length - 1));
			if (_targetDir != Dir && _targetDir != Direction.Transitioning)
				break;
		}

		Dir = Direction.Transitioning;

		//Debug.Log("SpotlightBoss.RandomTurn: " + _targetDir);

		Orient();
	}

	private void Orient()
	{
		Dir = Direction.Transitioning;
	}
}

