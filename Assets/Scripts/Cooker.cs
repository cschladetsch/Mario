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

	private bool _cooking;

	/// <summary>
	/// The buttons in this cooker
	/// </summary>
	private Dictionary<IngredientType, IngredientItem> _ingredientButtons;
	
	/// <summary>
	/// What is currently in the cooker
	/// </summary>
	private readonly Dictionary<IngredientType, int> _inventory = IngredientItem.CreateIngredientDict<int>();

	private readonly Dictionary<IngredientType, int> _requirements = IngredientItem.CreateIngredientDict<int>();

	protected override void Begin()
	{
		//Debug.Log("Cooker " + name + " Begin");
		base.Begin();

		World.GoalChanged += GoalChanged;

		ProgressBar.TotalTime = Recipe.CookingTime;
		ProgressBar.Reset();

		GatherRequirements();

		GatherIngredientButtons();
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

		if (CanCook())
			Cook();

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

	public void Pressed(GameObject go)
	{
		//Debug.Log("Cooker ingredient button pressed");
		var item = go.GetComponent<IngredientItem>();
		if (item == null)
		{
			Debug.LogWarning("GameObject " + go.name + " has no IngredientItem component in cooker for " + Recipe.Result);
			return;
		}

		var type = item.Type;
		if (Player.GetItemCount(type) == 0)
			return;

		_inventory[type]++;

		//Debug.Log("Added a " + type);

		item.SetAmount(_inventory[type], _inventory[type] >= _requirements[type]);

		Player.RemoveItem(type);

		UpdateDisplay();
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

	void OnDestroy()
	{
		//Debug.Log("Cooker " + name + " Destroyed");
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		//GatherIngredientButtons();
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

	/// <summary>
	/// Add an ingredient to the recipe
	/// </summary>
	/// <param name="type">the ingredient to add</param>
	/// <returns>true if was added</returns>
	public bool Add(IngredientType type)
	{
		//Debug.Log("Adding " + type + " to " + name);

		//for (var n = 0; n < Recipe.Inventory.Count; ++n)
		//{
		//	if (type != Recipe.Inventory[n]) 
		//		continue;

		//	_ingredients[type]++;
		//	_ingredientButtons[type].text = _ingredients[type].ToString();

		//	return true;
		//}

		return false;
	}

	/// <summary>
	/// Remove an ingredient from the recipe
	/// </summary>
	/// <param name="type"></param>
	/// <returns>true if ingredient was removed</returns>
	public bool Remove(IngredientType type)
	{
		//if (_ingredients[type] == 0)
		//	return false;
		//_ingredients[type]--;
		return true;
	}

	public IFuture<bool> Cook()
	{
		if (_cooking)
			return null;

		if (!CanCook())
			return null;

		// TODO: WHY OH WHY IS this.Kernel null?
		var k = FindObjectOfType<World>().Kernel;
		var future = k.Factory.NewFuture<bool>();
		Generator = k.Factory.NewCoroutine(Cook, future);
		return future;
	}

	public bool CanCook()
	{
		return !_cooking && Recipe.Satisfied(_inventory);
	}

	private IEnumerator Cook(IGenerator self, IFuture<bool> done)
	{
		if (_cooking)
		{
			self.Complete();
			yield break;
		}

		ProgressBar.Reset();
		ProgressBar.TotalTime = Recipe.CookingTime;

		//Debug.Log("Cooking a " + Recipe.Result);
		Product.interactable = false;
		_cooking = true;

		var remaining = Recipe.CookingTime;
		while (remaining > 0)
		{
			remaining -= (float) RealDeltaTime;
			ProgressBar.SetPercent(remaining/Recipe.CookingTime);
			yield return 0;
		}

		done.Value = true;

		self.Complete();

		EndCook();
	}

	private void EndCook()
	{
		Product.interactable = true;

		RemoveItemsFromInventory();

		Generator = null;

		if (Completed != null)
			Completed(Recipe.Result);

		ProgressBar.Reset();

		ResetRequiredIngredientsButton();

		World.Kernel.Factory.NewCoroutine(MoveCookedItems);

		UpdateDisplay();

		_cooking = false;
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

	void MovedToSellingArea(IngredientType type)
	{
		Player.CookedItem(type, 1);
	}

	IEnumerator MoveCookedItems(IGenerator self)
	{
		for (var i = 0; i < Recipe.NumResults; i++)
		{
			var area = World.Areas[AreaType.Bakery] as BakeryArea;
			ItemAnimation.Animate(Recipe.Result, Product.gameObject, area.SellingProductsPanel.GetProduct(Recipe.Result), 2, MovedToSellingArea);
			yield return self.ResumeAfter(TimeSpan.FromSeconds(0.5f));
		}

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

	public void IngredientButtonPressed(IngredientType item)
	{
		Debug.Log("Cooker added " + item);
		// TODO
		//if (Recipe.CanAdd(item))
		//	Recipe.Add(item);
	}

	public void CanUse(bool use)
	{
		Debug.Log("Can use cooker " + name + ": " + use);
	}

	public void StartBake()
	{
		Debug.Log("Start Bake");
		Cook();
	}

	public bool Cooking()
	{
		return _cooking;
	}
}