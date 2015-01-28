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

	private UiCanvas _canvas;

	void Update()
	{
	}

	protected override void Construct()
	{
		Control = GetComponent<Control>();
	}

	protected override void Begin()
	{
		_canvas = FindObjectOfType<UiCanvas>();
	}

	private int _lives = 3;

	public bool Dead { get { return _lives > 0; } }

	public void DroppedCake()
	{
		Debug.Log("Dropped cake " + _lives);

		if (Dead)
			return;

		_lives--;

		UpdateUi();
	}

	private void UpdateUi()
	{
		_canvas.LivesRemaining.text = _lives.ToString();
	}

	public void Reset()
	{
		_lives = 3;

		//Destroy(LeftCharacter);
		//Destroy(RightCharacter);

		//LeftCharacter = (GameObject) Instantiate(CharacterLeftPrefab);
		//RightCharacter = (GameObject) Instantiate(CharacterRightPrefab);
	}
}


