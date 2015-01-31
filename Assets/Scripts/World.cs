using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public Level Level;

	void Awake()
	{
		Level = FindObjectOfType<Level>();
	}

	void Start()
	{
		Level.BeginLevel();
	}

	void Update()
	{
	}

	private bool _paused;

	public void Pause(bool pause)
	{
		Debug.Log("Pause " + pause);

		_paused = pause;

		foreach (var cake in FindObjectsOfType<Cake>())
			cake.Pause(pause);

		Level.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}
}


