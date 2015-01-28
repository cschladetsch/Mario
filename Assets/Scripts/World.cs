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
}


