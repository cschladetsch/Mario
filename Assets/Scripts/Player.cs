using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The single player of the game, whom controls both Left and Right characters.
/// 
/// Keeps track of score and lives and pickups.
/// </summary>
public class Player : MarioObject
{
	public StageGoal CurrentGoal;

	public Text LivesRemainingText;

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

	///// <summary>
	///// The control (input) system
	///// </summary>
	//public Control Control;

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
	public bool Dead
	{
		get { return Lives == 0; }
	}

	/// <summary>
	/// If true, the player doesn't lose a life if a cake is dropped or hits a bomb
	/// </summary>
	public bool GodMode = false;

	/// <summary>
	/// Total current list of Inventory that the player has
	/// </summary>
	public Dictionary<IngredientType, int> Inventory = new Dictionary<IngredientType, int>();

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

	//private ProductsPanelScript _products;

	private Dictionary<IngredientType, int> _sold;

	protected override void Begin()
	{
		base.Begin();

		//_products = FindObjectOfType<ProductsPanelScript>();

		_sold = IngredientItem.CreateIngredientDict<int>();

		if (World.GodMode)
		{
			Player.Inventory[IngredientType.Cherry] = 99;
			Player.Inventory[IngredientType.Muffin] = 99;
			Player.Inventory[IngredientType.Mint] = 99;
			Player.Inventory[IngredientType.Chocolate] = 99;
		}
	}

	public void ShowCharacters(bool show)
	{
		if (!Left)
			return;

		AreaBase.SetVisual(Left.gameObject, show);
		AreaBase.SetVisual(Right.gameObject, show);
	}

	protected override void Construct()
	{
		//Debug.Log("Player.Construct");

		//Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();

		PrepareEmptyInventory();
	}

	private void PrepareEmptyInventory()
	{
		foreach (var e in Enum.GetValues(typeof (IngredientType)))
			Inventory.Add((IngredientType) e, 0);
	}

	protected override void BeforeFirstUpdate()
	{
		// TODO: attach to truck
		ShowCharacters(false);
	}

	private bool _lastAny;

	protected override void Tick()
	{
		base.Tick();

		// TODO: not test this every frame!


		GetCharacters();

#if !FINAL
		UpdateDebugKeys();
#endif
	}

	private void GetCharacters()
	{
		if (Left)
			return;

		Left = transform.FindChild("CharacterLeft").GetComponent<Character>();
		Right = transform.FindChild("CharacterRight").GetComponent<Character>();
	}

	private void UpdateDebugKeys()
	{
#if DEBUG
		if (!Input.GetKeyDown(KeyCode.LeftShift))
			return;

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
#endif
	}

	public void DroppedCake(Pickup pickup)
	{
		if (Dead || GodMode)
			return;

		if (!Cake.IsCake(pickup.Type)) 
			return;

		//Debug.Log("Dropped a " + pickup.name);
		if (OnCakeDropped != null)
			OnCakeDropped(this);

		LoseLife();
	}

	private void LoseLife()
	{
		if (--Lives == 0)
			Died();

		UpdateUi();
	}

	private void Died()
	{
		World.Pause(true);

		Canvas.GameOverPanel.gameObject.SetActive(true);

		if (!_canvas.Score)
			return;

		var score = int.Parse(_canvas.Score.text);

		if (SaveGame.UpdateHighScore(score))
			_canvas.ShowHighScore(score);
		else
			_canvas.ShowTapToStart();
	}

	public void UpdateUi()
	{
		World.Canvas.UpdateGoldAmount();
		World.Canvas.GoalPanel.GetComponent<GoalPanel>().UpdateUi();
		World.CookingAreaUi.InventoryPanel.UpdateDisplay(Inventory, false);
		World.CookingAreaUi.ProductsPanel.UpdateUi();

		LivesRemainingText.text = Lives.ToString();
	}

	public void Reset()
	{
		Lives = 3;
		Gold = 5;

		Inventory = IngredientItem.CreateIngredientDict<int>();
		foreach (var cooker in World.BakeryArea.gameObject.GetComponentsInChildren<Cooker>())
			cooker.Reset();

		UpdateUi();
	}

	public void BombHit(Bomb bomb)
	{
		Debug.Log("Player hit bomb");
		LoseLife();
	}

	public void AddLife()
	{
		//Debug.Log("Player.AddLife");

		++Lives;

		UpdateUi();
	}

	public void CookedItem(IngredientType type, int count)
	{
		//Debug.Log("Player.CookedItem: " + type);
		Inventory[type] += count;
		UpdateUi();
	}

	/// <summary>
	/// Check if we have reached current goal
	/// </summary>
	/// <returns></returns>
	public bool GoalReached()
	{
		//Debug.Log("Player.GoalReached?");
		// make a dictionary mapping type to number required
		var dict = IngredientItem.CreateIngredientDict<int>();
		foreach (var type in CurrentGoal.Ingredients)
			dict[type]++;

		// check that we sold enough of the things
		foreach (var kv in _sold)
		{
			if (!dict.ContainsKey(kv.Key))
				continue;

			var needed = dict[kv.Key];
			if (kv.Value < needed && needed > 0)
			{
				//Debug.Log("Not enough " + kv.Key + ": need " + needed + ", have " + kv.Value);
				return false;
			}
		}

		// reset sold for next goal
		foreach (var kv in dict)
			_sold[kv.Key] = 0;

		return true;
	}

	public void SetGoal(StageGoal goal)
	{
		CurrentGoal = goal;
	}

	public void AddCake(Cake cake)
	{
		AddCake(cake.Type);
		UpdateUi();
	}

	public bool RemoveItem(IngredientType type)
	{
		if (Inventory[type] == 0)
			return false;

		Inventory[type]--;

		foreach (var panel in FindObjectsOfType<InventoryPanel>())
			panel.UpdateDisplay(Inventory, false);

		return true;
	}

	public int GetItemCount(IngredientType type)
	{
		return Inventory[type];
	}

	public bool HasItem(IngredientType type)
	{
		return Inventory[type] > 0;
	}

	public void SoldItem(IngredientInfo info)
	{
		if (Inventory[info.Type] == 0)
			return;

		//Debug.Log("Sold a " + info.Type + " @" + Time.frameCount);
		Gold += info.Sell;
		_sold[info.Type]++;
		Inventory[info.Type]--;

		UpdateUi();
	}

	public void RemoveGold(int sum)
	{
		Gold -= sum;
		UpdateUi();
	}

	public void AddCake(IngredientType type)
	{
		Inventory[type]++;
		UpdateUi();
	}

	public void AddGold(int amount)
	{
		//Debug.Log("Player added " + amount + " gold");
		Gold += amount;
		UpdateUi();
	}
}