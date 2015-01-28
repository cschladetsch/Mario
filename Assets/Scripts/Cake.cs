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

	void Update()
	{
		UpdateHang();
	}

	private void UpdateHang()
	{
		if (_hangTimer > 0)
		{
			_hangTimer -= Time.deltaTime;
			if (_hangTimer < 0)
				StartDropped();
		}
	}

	private void StartDropped()
	{
		Debug.Log("Dropped cake");
		Destroy(gameObject);
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
	}
}


