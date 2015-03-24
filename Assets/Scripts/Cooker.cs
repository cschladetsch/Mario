using System.Collections.Generic;
using System.Linq;
using System;
using Flow;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Something that makes a product given correct ingredients
/// </summary>
public class Cooker : MarioObject
{
	/// <summary>
	/// How to make the product that this cooker... cooks
	/// </summary>
	public Recipe Recipe;

	public Button Product;

	/// <summary>
	/// Used to disable the cooker
	/// </summary>
	public GameObject Overlay;

	public ProgressBar ProgressBar;

	public new IKernel Kernel;

	public Color DeselectedColor;

	public Color UnusableColor;

	/// <summary>
	/// True if this is currently selected cooker
	/// </summary>
	public bool Active;

	public IGenerator Generator;

	public IFuture<bool> Future;

	public delegate void CompletedHandler(IngredientType type);

	public event CompletedHandler Completed;

	public UnityEngine.UI.Image ProductImage;

	/// <summary>
	/// The buttons in this cooker
	/// </summary>
	private Dictionary<IngredientType, IngredientItem> _ingredientButtons;

	/// <summary>
	/// What is currently in the cooker
	/// </summary>
	private Dictionary<IngredientType, int> _inventory = IngredientItem.CreateIngredientDict<int>();

	private readonly Dictionary<IngredientType, int> _requirements = IngredientItem.CreateIngredientDict<int>();

	protected override void Begin()
	{
		//Debug.Log("Cooker " + name + " Begin");
		base.Begin();

		World.GoalChanged += GoalChanged;

		ProgressBar.TotalTime = Recipe.CookingTime;
		ProgressBar.Reset();
		ProgressBar.Ended -= ProgressEnded;
		ProgressBar.Ended += ProgressEnded;

		GatherRequirements();

		GatherIngredientButtons();
	}

	/// <summary>
	/// The progress bar has ended - but it may've been hours or days since
	/// it was started. we need to produce and maybe even sell everything
	/// that may've been made while the device was off or game suspended
	/// </summary>
	/// <param name="pb"></param>
	private void ProgressEnded(ProgressBar pb)
	{
		var info = World.GetInfo(Recipe.Result);
		var elapsed = pb.Elapsed;
		var numCooked = 0;

		var cookingTime = Recipe.CookingTime;
		var numResults = Recipe.NumResults;

		//Debug.Log("Cooker.ProgressEnded: " + pb.Elapsed + ", cookingTime: " + cookingTime + " selling time:" + info.SellingTime);

		// find out how many we cooked
		while (elapsed > cookingTime)
		{
			if (!Recipe.Satisfied(_inventory))
				break;

			RemoveItemsFromInventory();

			numCooked += numResults;
			elapsed -= cookingTime;
		}

		//Debug.Log("Cooked " + numCooked);

		// now immediately sell using whatever time remaining
		var sellTime = info.SellingTime;
		while (numCooked > 0 && elapsed > sellTime)
		{
			Player.AddCake(info.Type);
			World.CurrentArea.SellItem(info.Type);
			elapsed -= sellTime;
			--numCooked;
		}

		//Debug.Log("Adding  remainder " + numCooked + " of type " + Recipe.Result + " to selling area...");

		// TODO WTF why is Kernel empty
		var k = FindObjectOfType<World>().Kernel;
		k.Factory.NewCoroutine(MoveCookedItems, numCooked);

		Product.interactable = true;

		ProgressBar.Reset();

		UpdateDisplay();
		Player.UpdateUi();

		foreach (var kv in _inventory)
			if (kv.Value > 0)
				Debug.Log(string.Format("Have {1} of {0} left", kv.Value, kv.Key));
	}

	private void GatherRequirements()
	{
		for (int i = 0; i < Recipe.Ingredients.Count; i++)
			_requirements[Recipe.Ingredients[i]] = Recipe.Counts[i];
	}

	private void GoalChanged(int index, StageGoal newgoal)
	{
	}

	public void AddIngredient()
	{
		foreach (var kv in _requirements)
		{
			var type = kv.Key;
			var required = kv.Value;

			if (_inventory[type] >= required || !Player.HasItem(type))
				continue;

			_inventory[type]++;
			Player.RemoveItem(type);
			var source = FindObjectOfType<InventoryPanel>().GetButton(type);
			ItemAnimation.Animate(type, source, Product.gameObject, 1);
			break;
		}

		//Debug.Log(string.Format("ProgressBasePaused: {0}, Elapsed {1}, CanCook {2}", ProgressBar.Paused, ProgressBar.Elapsed, CanCook()));
		if (ProgressBar.Paused && CanCook())
		{
			ProgressBar.Reset();
			ProgressBar.Paused = false;
		}

		UpdateDisplay();
	}

	private GameObject FindButton(IngredientType type)
	{
		return _ingredientButtons.ContainsKey(type) ? _ingredientButtons[type].gameObject : null;
	}

	protected override void Tick()
	{
		base.Tick();

		Overlay.SetActive(!IsInteractable());
	}

	private bool IsInteractable()
	{
		for (var i = 0; i <= Mathf.Min(World.GoalIndex, World.StageGoals.Length - 1); i++)
		{
			var goal = World.StageGoals[i];

			//Debug.Log("Testing for goal: " + goal.Name);
			// if this cooker can make any of the requirements in any of the goals,
			// then it is interactable
			if (goal.Ingredients.Any(req => Recipe.Result == req))
			{
				//Debug.Log("Can use " + Recipe.Result);
				return true;
			}
		}

		return false;
	}

	public void Select(bool select)
	{
		Debug.Log("Selected " + name + " " + select);
	}

	private void OnDestroy()
	{
		//Debug.Log("Cooker " + name + " Destroyed");
	}

	private void GatherIngredientButtons()
	{
		//Debug.Log("GatherIngredientButtons for " + name);

		_ingredientButtons = IngredientItem.CreateIngredientDict<IngredientItem>();
		foreach (var item in transform.GetComponentsInChildren<IngredientItem>())
		{
			if (item == null)
				return;

			_ingredientButtons[item.Type] = item;
			var amount = _requirements[item.Type];
			//Debug.Log(String.Format("amount {0}, type {1}", amount, item.Type));
			item.SetAmount(amount, false);
		}

		var list = (from kv in _ingredientButtons where kv.Value == null select kv.Key).ToList();
		foreach (var k in list)
			_ingredientButtons.Remove(k);
	}

	public void Cook()
	{
		if (!CanCook())
			return;

		ProgressBar.Reset();
		ProgressBar.Paused = false;
	}

	public bool CanCook()
	{
		return ProgressBar.Paused && Recipe.Satisfied(_inventory);
	}

	private void ResetRequiredIngredientsButton()
	{
		foreach (var kv in _ingredientButtons)
		{
			var type = kv.Key;
			var button = kv.Value;
			//Debug.Log(string.Format("Buttton {0} needs {1}", button.Type, _requirements[type]));
			button.SetAmount(_requirements[type], false);
		}
	}

	private void MovedToSellingArea(IngredientType type)
	{
		Player.CookedItem(type, 1);
	}

	private IEnumerator MoveCookedItems(IGenerator self, int num)
	{
		var product = World.BakeryArea.SellingProductsPanel.GetProduct(Recipe.Result);
		for (var i = 0; i < num; i++)
		{
			if (World.CurrentArea != World.BakeryArea)
			{
				Player.CookedItem(Recipe.Result, 1);
			}
			else
			{
				// don't animate a cooked item if we're not in the bakery area
				ItemAnimation.Animate(Recipe.Result, Product.gameObject, product, 2, MovedToSellingArea);
			}

			yield return self.ResumeAfter(TimeSpan.FromSeconds(UnityEngine.Random.Range(.5f, 1.0f)));
		}

		self.Complete();
	}

	private void RemoveItemsFromInventory()
	{
		for (int i = 0; i < Recipe.Ingredients.Count; i++)
			_inventory[Recipe.Ingredients[i]] -= Recipe.Counts[i];
	}

	private void UpdateDisplay()
	{
		UpdateIngredientButtons();

		foreach (var ing in FindObjectsOfType<InventoryPanel>())
			ing.UpdateDisplay(Player.Inventory, false);
	}

	private bool UpdateIngredientButtons()
	{
		//Debug.Log("UpdateIngredientButtons: " + _ingredientButtons.Count + " for " + name);
		foreach (var kv in _ingredientButtons)
		{
			var button = kv.Value;
			if (button == null)
				continue;

			var amount = _inventory[button.Type];
			var required = _requirements[button.Type];
			var avail = amount >= required;
			var num = required - amount;

			//Debug.Log(string.Format("amount {0}, required {1}, num {2}, avail {3}", amount, required, num, avail));
			button.SetAmount(num, avail);
		}

		return false;
	}

	public void CanUse(bool use)
	{
		Debug.Log("Can use cooker " + name + ": " + use);
	}

	public void Reset()
	{
		_inventory = IngredientItem.CreateIngredientDict<int>();
		ProgressBar.Reset();
	}
}