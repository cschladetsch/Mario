using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public Level Level;

	private bool _paused;

	void Awake()
	{
		Application.targetFrameRate = 60;
		Level = FindObjectOfType<Level>();
	}

	void Start()
	{
		Pause(true);
	}

	void Update()
	{
	}

	public void Pause(bool pause)
	{
		_paused = pause;

		foreach (var cake in FindObjectsOfType<Cake>())
			cake.Pause(pause);

		Level.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}

	public void StartGame()
	{
		Pause(false);
		Level.BeginLevel();
	}
}


