using System;
using UnityEngine;

/// <summary>
/// The Stoplight Boss logic.
/// 
/// When facing Left, the left character has its controls inverted.
///
/// When facing Right, the right character has its controls inverted.
/// </summary>
public class StoplightBoss : Boss
{
	public enum Direction
	{
		Left, Right, Forward, Back, Transitioning
	}

	/// <summary>
	/// The direction the boss is facing
	/// </summary>
	public Direction Dir;

	/// <summary>
	/// The minimum time between changes in orientation of the boss
	/// </summary>
	public float MinChangeTime = 5;

	/// <summary>
	/// The maximum time between changes in orientation of the boss
	/// </summary>
	public float MaxChangeTime = 10;

	/// <summary>
	/// Min time to take to transition between orientations
	/// </summary>
	public float MinTransitionTime = 1;

	/// <summary>
	/// Max time to take to transition between orientations
	/// </summary>
	public float MaxTransitionTime = 2;

	/// <summary>
	/// Timer for current change transition
	/// </summary>
	private float _transitionTimer;

	/// <summary>
	/// How long till next change
	/// </summary>
	private float _changeTimer;

	/// <summary>
	/// The different orientations in space
	/// </summary>
	private Transform _left, _right, _forward, _back;

	private Vector3 _startPos;

	private Quaternion _startRot;

	private Transform _target;

	private Transform _visual;

	protected override void Begin()
	{
		Reset();
	}

	public void Reset()
	{
		base.Begin();

		ResetChangeTimer();

		GetOrientations();

		RandomTurn();
	}

	private void GetOrientations()
	{
		_visual = transform.FindChild("Pivot");
	}

	private void ResetChangeTimer()
	{
		_changeTimer = UnityEngine.Random.Range(MinChangeTime, MaxChangeTime);
	}

	protected override void Tick()
	{
		base.Tick();

		// change orientation
		ChangeDirection();
	}

	/// <summary>
	/// Determine if we need to change orientation, and start a transition if so
	/// </summary>
	private void ChangeDirection()
	{
		_changeTimer -= DeltaTime;
		if (_changeTimer > 0)
			return;

		ResetChangeTimer();

		RandomTurn();
	}

	private void RandomTurn()
	{
		var values = Enum.GetValues(typeof(Direction));
		var cur = Dir;

		// ensure we transition to a new direction
		while (true)
		{
			Dir = (Direction) values.GetValue(UnityEngine.Random.Range(0, values.Length - 1));
			if (Dir != cur)
				break;
		}

		Debug.Log("SpotlightBoss.RandomTurn: " + Dir);
	}
}

