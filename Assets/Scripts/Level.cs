using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : HasWorld
{
	public GameObject CakePrefab;

	void Update()
	{
	}

	public void BeginLevel()
	{
		Debug.Log("Level begins");

		Reset();

		Player.Reset();
	}

	private void Reset()
	{
	}
}
