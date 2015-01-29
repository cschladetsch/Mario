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

	public bool Dead { get { return _lives == 0; } }

	public delegate void CollisionHandler(Collision2D other);
	public delegate void TriggerHandler(Collider2D other);
	public delegate void PlayerEventHandler(Player player);

	public CollisionHandler OnCollision;
	public TriggerHandler OnTrigger;
	public PlayerEventHandler OnDied;

	private UiCanvas _canvas;

	private int _lives = 3;

	void Update()
	{
	}

	protected override void Construct()
	{
		//Debug.Log("Player.Construct");
		Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();
	}

	public void DroppedCake()
	{
		//Debug.Log("Dropped cake " + _lives);

		if (Dead)
			return;

		if (--_lives == 0)
			Died();

		UpdateUi();
	}

	private void Died()
	{
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


