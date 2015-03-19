using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Flow;
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

	private ProductsPanelScript _products;

	private Dictionary<IngredientType, int> _sold;

	protected override void Begin()
	{
		base.Begin();

		_products = FindObjectOfType<ProductsPanelScript>();
		_sold = IngredientItem.CreateIngredientDict<int>();
	}

	public void ShowCharacters(bool show)
	{
		if (!Left)
			return;

		AreaBase.ToggleVisuals(Left.gameObject, show);
		AreaBase.ToggleVisuals(Right.gameObject, show);
	}

	protected override void Construct()
	{
		//Debug.Log("Player.Construct");

		Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();

		PrepareEmptyInventory();

		//Inventory[IngredientType.Cherry] = 5;
		//Inventory[IngredientType.Muffin] = 5;
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

		UpdateSellingProgressBar();

		GetCharacters();

#if !FINAL
		UpdateDebugKeys();
#endif

		UpdateSellItem();
	}

	private void GetCharacters()
	{
		if (Left)
			return;

		Left = transform.FindChild("CharacterLeft").GetComponent<Character>();
		Right = transform.FindChild("CharacterRight").GetComponent<Character>();
	}

	private void UpdateSellingProgressBar()
	{
		var any = Inventory[IngredientType.CupCake] > 0 || Inventory[IngredientType.MintIceCream] > 0;
		var bar = _products.SellProgressBar;
		if (!any && !bar.Paused)
		{
			bar.Reset();
		}

		if (any && bar.Paused || (!_lastAny && any))
		{
			bar.Reset();
			bar.Paused = false;

			SellingTimer = SellingInterval;

			// TODO: Use Recipe.SellingTime
			bar.TotalTime = Player.SellingInterval;
		}

		_lastAny = any;
	}

	private void UpdateDebugKeys()
	{
#if DEBUG
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

	private List<Product> CalcPossibleProducts()
	{
		return null;
	}

	public void DroppedCake(Pickup pickup)
	{
		if (Dead || GodMode)
			return;

		if (pickup is Cake)
		{
			Debug.Log("Dropped a " + pickup.name);

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

	public float SellingTimer;

	private void UpdateSellItem()
	{
		SellingTimer -= (float) RealDeltaTime;

		var selling = SellingTimer <= 0;
		while (selling)
		{
			SellItem();
			SellingTimer += SellingInterval;

			if (SellingTimer > 0)
			{
				selling = false;
				SellingTimer = SellingInterval;
			}
		}
	}

	private void SellItem()
	{
		IngredientType[] types = {IngredientType.MintIceCream, IngredientType.CupCake};

		var canSell = false;
		foreach (var ty in types)
		{
			if (Inventory[ty] > 0)
			{
				canSell = true;
				break;
			}
		}

		if (!canSell)
			return;

		while (true)
		{
			var index0 = UnityEngine.Random.Range(0, types.Length);
			var type = types[index0];

			if (Inventory[type] <= 0)
				continue;

			SellItem(type);

			break;
		}
	}

	private void SellItem(IngredientType type)
	{
		if (Inventory[type] == 0)
			return;

		var info = World.IngredientInfo[type];
		Gold += info.Sell;
		Inventory[type]--;
		//Debug.Log("SOLD a " + type + " for " + info.Sell + "$" + UnityEngine.Time.frameCount);
		_sold[type]++;

		var p = FindObjectOfType<ProductsPanelScript>();
		if (p)
			p.SellProgressBar.Reset();

		UpdateUi();

		World.GoalPanel.AddItem(type);

		if (GoalReached())
			World.NextGoal();
	}

	//IEnumerator MoveSoldItem(IGenerator self, IngredientType type)
	//{
	//	//var info = World.GetInfo(type);
	//	//var image = info.Image;
	//	//var sprite = new UI.Sprite();
	//	//sprite.
	//}

	private void Died()
	{
		World.Pause(true);

		if (!_canvas.Score)
			return;

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
		World.CookingAreaUi.InventoryPanel.UpdateDisplay(Inventory, false);

		LivesRemainingText.text = Lives.ToString();
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
		//Debug.Log("Player.CookedItem: " + type);
		Inventory[type] += count;

		UpdateUi();
	}

	/// <summary>
	/// Check if we have reached current goal
	/// </summary>
	/// <returns></returns>
	private bool GoalReached()
	{
		Debug.Log("Player.GoalReached?");
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
				Debug.Log("Not enough " + kv.Key + ": need " + needed + ", have " + kv.Value);
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
		Inventory[cake.Type]++;
		UpdateUi();
	}
}