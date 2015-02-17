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
	/// The total duration of the current transition
	/// </summary>
	private float _transitionTime;

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

		//_visual.transform.position = _forward.position;
		//_visual.transform.rotation = _forward.rotation;

		RandomTurn();
	}

	private void GetOrientations()
	{
		_visual = transform.FindChild("Pivot");
		//var orientations = transform.FindChild("Orientations");
		//_left = orientations.FindChild("Left");
		//_right = orientations.FindChild("Right");
		//_forward = orientations.FindChild("Forward");
		//_back = orientations.FindChild("Back");

		//_left.gameObject.SetActive(false);
		//_right.gameObject.SetActive(false);
		//_forward.gameObject.SetActive(false);
		//_back.gameObject.SetActive(false);
	}

	private void ResetChangeTimer()
	{
		_changeTimer = UnityEngine.Random.Range(MinChangeTime, MaxChangeTime);
	}

	protected override void Tick()
	{
		base.Tick();

		// move between orientations
		UpdateTransition();

		// change orientation
		ChangeDirection();
	}

	/// <summary>
	/// Update the transition from one orientation to the next
	/// </summary>
	private void UpdateTransition()
	{
		// no longer transitioning to new orientation
		if (Dir != Direction.Transitioning)
			return;


		_transitionTimer -= DeltaTime;

		var t = 1.0f - (_transitionTimer/_transitionTime);		// 0..1
		_visual.transform.position = _startPos + (_target.position - _startPos)*t;
		_visual.transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, t);

		//_visual.transform.rotation = _target.rotation;

		//Debug.Log("Transitioning: " + t + ", " + transform.rotation);
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

		//Orient();
	}

	//private void Orient()
	//{
	//	Transform tr = null;

	//	switch (Dir)
	//	{
	//		case Direction.Left:
	//			tr = _left;
	//			break;

	//		case Direction.Right:
	//			tr = _right;
	//			break;

	//		case Direction.Forward:
	//			tr = _back;
	//			break;

	//		case Direction.Back:
	//			tr = _forward;
	//			break;

	//		case Direction.Transitioning:
	//			Debug.LogError("Can't orient when transitioning");
	//			break;
	//	}

	//	_transitionTimer = _transitionTime = UnityEngine.Random.Range(MinTransitionTime, MaxTransitionTime);

	//	_startPos = _visual.transform.position;
	//	_startRot = _visual.transform.rotation;

	//	_target = tr;

	//	Dir = Direction.Transitioning;
	//}
}

