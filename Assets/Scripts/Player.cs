using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : HasWorld
{
	public GameObject CharacterLeftPrefab;

	public GameObject CharacterRightPrefab;

	public Control Control;

	internal GameObject LeftCharacter, RightCharacter;

	internal GameObject EmittingParticleSystems;

	public GameObject Effects;

	public delegate void CollisionHandler(Collision2D other);
	public delegate void TriggerHandler(Collider2D other);
	public delegate void PlayerEventHandler(Player player);

	public CollisionHandler OnCollision;
	public TriggerHandler OnTrigger;
	public PlayerEventHandler OnDied;

	void Update()
	{
	}

	protected override void Construct()
	{
		Control = GetComponent<Control>();
	}

	protected override void Begin()
	{
	}

	public void Reset()
	{
		//Destroy(LeftCharacter);
		//Destroy(RightCharacter);

		//LeftCharacter = (GameObject) Instantiate(CharacterLeftPrefab);
		//RightCharacter = (GameObject) Instantiate(CharacterRightPrefab);
	}
}


