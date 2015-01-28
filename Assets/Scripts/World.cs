using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	private Player Player;
	
	void Awake()
	{
		Player = FindObjectOfType<Player>();
	}

	void Start()
	{
	}

	void Update()
	{
	}
}


