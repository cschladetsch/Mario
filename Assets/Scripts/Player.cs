﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;

/// <summary>
/// The single player of the game, whom controls both Left and Right characters.
/// 
/// Keeps track of score and lives and pickups.
/// </summary>
public class Player : MarioObject
{
	public StageGoal CurrentGoal;

	/// <summary>
	/// How much gold (money) the player has
	/// </summary>
	public int Gold = 5;

	/// <summary>
	/// How to make the left character
	/// </summary>
	public GameObject CharacterLeftPrefab;

	/// <summary>
	/// How to make the right character
	/// </summary>
	public GameObject CharacterRightPrefab;

	/// <summary>
	/// The control (input) system
	/// </summary>
	public Control Control;

	/// <summary>
	/// Where to store particle systems
	/// </summary>
	internal GameObject EmittingParticleSystems;

	/// <summary>
	/// The set of effects used by the player
	/// </summary>
	public GameObject Effects;

	/// <summary>
	/// If dead, the player cannot play anymore. Fact of life.
	/// </summary>
	public bool Dead { get { return Lives == 0; } }

	/// <summary>
	/// If true, the player doesn't lose a life if a cake is dropped or hits a bomb
	/// </summary>
	public bool GodMode = false;

	/// <summary>
	/// Total current list of Ingredients that the player has
	/// </summary>
	public Dictionary<IngredientType, int> Ingredients = new Dictionary<IngredientType, int>();

	///// <summary>
	///// The completed products - they may be sold directly, or used to make better products
	///// </summary>
	//public List<Product> Products = new List<Product>();

	///// <summary>
	///// What is currently on sale
	///// </summary>
	//public List<Product> SellingProducts = new List<Product>(); 

	public delegate void CollisionHandler(Collision2D other);
	public delegate void TriggerHandler(Collider2D other);
	public delegate void PlayerEventHandler(Player player);

	public CollisionHandler OnCollision;
	public TriggerHandler OnTrigger;
	public PlayerEventHandler OnDied;
	public PlayerEventHandler OnCakeDropped;

	/// <summary>
	/// The left character
	/// </summary>
	public Character Left;

	/// <summary>
	/// The right character
	/// </summary>
	public Character Right;

	/// <summary>
	/// Cached access to the UI
	/// </summary>
	private UiCanvas _canvas;

	/// <summary>
	/// When a cake is dropped or you hit a bomb, you lose a life. 0 lives = game over
	/// </summary>
	public int Lives = 3;

	public void ShowCharacters(bool show)
	{
		if (Left)
			Left.gameObject.SetActive(!show);

		if (Right)
			Right.gameObject.SetActive(!show);
	}

	protected override void Construct()
	{
		//Debug.Log("Player.Construct");

		Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();

		PrepareEmptyInventory();

		//Ingredients[IngredientType.Cherry] = 5;
		//Ingredients[IngredientType.Muffin] = 5;
	}

	private void PrepareEmptyInventory()
	{
		foreach (var e in Enum.GetValues(typeof (IngredientType)))
			Ingredients.Add((IngredientType) e, 0);
	}

	protected override void BeforeFirstUpdate()
	{
		// TODO: attach to truck
		Left = transform.FindChild("Left").GetComponent<Character>();
		Right = transform.FindChild("Right").GetComponent<Character>();

		ShowCharacters(false);
	}

	protected override void Tick()
	{
		base.Tick();

#if !FINAL
		UpdateDebugKeys();
#endif

		UpdateSellItem();
	}

	private void UpdateDebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FindObjectOfType<World>().TogglePause();
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			CookedItem(IngredientType.CupCake, 1);
			if (GoalReached())
				World.NextGoal();
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			CookedItem(IngredientType.MintIceCream, 1);
		}
	}

	List<Product> CalcPossibleProducts()
	{
		return null;
	}

	public void DroppedCake(Pickup pickup)
	{
		if (Dead || GodMode)
			return;

		if (pickup is Cake)
		{
			if (OnCakeDropped != null)
				OnCakeDropped(this);

			LoseLife();
		}
	}

	private void LoseLife()
	{
		if (--Lives == 0)
			Died();

		UpdateUi();
	}

	public float SellingInterval = 3;

	private float _sellingTimer;

	private void UpdateSellItem()
	{
		//Debug.Log("UpdateSellItem: " + _sellingTimer);

		_sellingTimer -= DeltaTime;

		if (_sellingTimer <= 0)
		{
			SellItem();
			_sellingTimer = SellingInterval;
		}
	}

	private void SellItem()
	{
		IngredientType[] types = {IngredientType.MintIceCream, IngredientType.CupCake};

		bool sold = false;
		foreach (var type in types)
		{
			if (Ingredients[type] > 0)
			{
				sold = true;
				SellItem(type);
				break;
			}
		}

		if (sold)
			UpdateUi();
	}

	private void SellItem(IngredientType type)
	{
		if (Ingredients[type] == 0)
			return;

		var info = World.IngredientInfo[type];
		Gold += info.Sell;
		Ingredients[type]--;
		Debug.LogWarning("Sold a " + type + " for " + info.Sell + "$");

		UpdateUi();
	}

	private void Died()
	{
		World.Pause(true);

		var score = int.Parse(_canvas.Score.text);

		if (SaveGame.UpdateHighScore(score))
			_canvas.ShowHighScore(score);
		else
			_canvas.ShowTapToStart();
	}

	private void UpdateUi()
	{
		World.Canvas.UpdateGoldAmount();
		World.Canvas.GoalPanel.GetComponent<GoalPanel>().UpdateUi();
		World.CookingAreaUi.InventoryPanel.UpdateDisplay(Ingredients, false);
		World.BuyingAreaUi.InventoryPanel.UpdateDisplay(Ingredients, false);

		// TODO
		//_canvas.LivesRemaining.text = Lives.ToString();
	}

	public void Reset()
	{
		Lives = 3;

		UpdateUi();
	}

	public void BombHit(Bomb bomb)
	{
		LoseLife();
	}

	public void AddLife()
	{
		++Lives;

		UpdateUi();
	}

	public void CookedItem(IngredientType type, int count)
	{
		Debug.Log("Player.CookedItem: " + type);
		Ingredients[type] += count;

		World.Canvas.GoalPanel.Cooked(type, count);

		if (GoalReached())
			World.NextGoal();

		UpdateUi();
	}

	private bool GoalReached()
	{
		var dict = IngredientItem.CreateIngredientDict<int>();
		foreach (var i in CurrentGoal.Ingredients)
		{
			dict[i]++;
		}

		foreach (var kv in Ingredients)
		{
			var needed = dict[kv.Key];
			if (kv.Value < needed && needed > 0)
			{
				//Debug.Log("Not enough " + kv.Key + ": need " + needed + ", have " + kv.Value);
				return false;
			}
		}

		return true;

	}

	public void SetGoal(StageGoal goal)
	{
		Debug.Log("Player.SetGoal: " + goal.Name);

		CurrentGoal = goal;
		var goalPanel = Canvas.GoalPanel.GetComponent<GoalPanel>();
		goalPanel.Refresh();
	}
}
