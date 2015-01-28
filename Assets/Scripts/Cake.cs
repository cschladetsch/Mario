using System;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
	/// <summary>
	/// Parameterised position along conveyor: 0 is at start, 1 at end
	/// </summary>
	public float Position;

	public float HangTime = 4;

	private float _hangTimer;

	public bool Hanging { get { return _hangTimer > 0; } }

	public bool Dropped { get { return _hangTimer < 0; } }
	
	void Awake()
	{
	}

	void Start()
	{
	}

	internal void UpdateCake(bool moveRight)
	{
		UpdateHang(moveRight);
	}

	private void UpdateHang(bool moveRight)
	{
		if (!(_hangTimer > 0))
			return;

		transform.localRotation = Quaternion.Slerp(transform.localRotation,
			Quaternion.AngleAxis(moveRight ? -30 : 30, Vector3.forward), Time.deltaTime);

		_hangTimer -= Time.deltaTime;
		if (_hangTimer < 0)
			StartDropped();
	}

	private void StartDropped()
	{
		//Destroy(gameObject);
		FindObjectOfType<Player>().DroppedCake();
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
	}

	public void Reset()
	{
		_hangTimer = 0;
		Position = 0;
		transform.localRotation = Quaternion.identity;
	}
}


