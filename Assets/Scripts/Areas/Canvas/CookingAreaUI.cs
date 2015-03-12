using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class CookingAreaUI : MarioObject
{
	/// <summary>
	/// The visuals for the current inventory in the canvas
	/// </summary>
	public InventoryPanel InventoryPanel;

	//public List<Cooker> Cookers;

	//public Cooker SelectedCooker;

	public UnityEngine.UI.Button ShopButton;
 
	public Cooker MintIceCreamCooker;

	public Cooker CupCakeCooker;

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		//GatherCookers();

		UpdateDisplay();
	}

	public void AddIngredient(GameObject ing)
	{
		var cooker = ing.transform.parent.GetComponent<Cooker>();
		var type = ing.GetComponent<IngredientItem>().Type;

		if (type == IngredientType.Mint || type == IngredientType.Chocolate)
			cooker = MintIceCreamCooker;
		else
			cooker = CupCakeCooker;


		if (Player.Inventory[type] > 0)
		{
			if (cooker.Add(type))
			{
				Player.Inventory[type]--;
				if (cooker.CanCook())
					cooker.Cook();

				UpdateDisplay();
			}
		}
	}

	public void ProductClicked(GameObject product)
	{
		var cooker = product.transform.parent.GetComponent<Cooker>();
		var recipe = cooker.Recipe;

		var canCook = true;
		var req = IngredientItem.CreateIngredientDict<int>();
		for (var n = 0; n < recipe.Ingredients.Count; ++n)
		{
			var ty = recipe.Ingredients[n];
			var num = recipe.Counts[n];

			if (Player.Inventory[ty] < num)
			{
				canCook = false;
				break;
			}

			req[ty] += num;
		}

		if (!canCook)
			return;

		Debug.Log("Cooking " + recipe.NumResults + " of " + recipe.Result);

		switch (recipe.Result)
		{
			case IngredientType.CupCake:
				if (CupCakeCooker.CanCook())
				{
					RemoveIngredients(req);
					CupCakeCooker.Cook();
				}
				break;

			case IngredientType.MintIceCream:
				if (MintIceCreamCooker.CanCook())
				{
					RemoveIngredients(req);
					MintIceCreamCooker.Cook();
				}
				break;
		}
	}

	private void RemoveIngredients(Dictionary<IngredientType, int> req)
	{
		foreach (var kv in req)
		{
			//Debug.Log("Removing " + kv.Value + " of " + kv.Key);
			Player.Inventory[kv.Key] -= kv.Value;
		}

		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	private void GatherCookers()
	{
		//foreach (Transform tr in transform)
		//{
		//	var cooker = tr.GetComponent<Cooker>();
		//	if (!cooker)
		//		continue;

		//	Cookers.Add(cooker);
		//	//Debug.Log("Found a cooker for " + cooker.Recipe.name);

		//	//  HACKS
		//	if (cooker.name == "CupcakeCooker")
		//		_cakes = cooker;
		//	else
		//		_iceCream = cooker;
		//}
	}

	protected override void Tick()
	{
		base.Tick();


		//if (CupCakeCooker.CanCook())

		//var cooking = !_cakes.Cooking() && !_iceCream.Cooking();
		//ShopButton.interactable = !cooking;
	}

	//private Cooker _cakes;
	//private Cooker _iceCream;

	public void IngredientButtonPressed(GameObject button)
	{
		//Debug.Log("Pressed " + button.name);
		//if (!SelectedCooker)
		//	return;
		var type = button.GetComponent<IngredientItem>().Type;
		if (Player.Inventory[type] == 0)
			return;

		//SelectedCooker.IngredientButtonPressed(item);

		// HACKS
		switch (type)
		{
			case IngredientType.Cherry:
			case IngredientType.Muffin:
			{
				if (CupCakeCooker.Add(type))
				{
					RemoveItemFromPlayer(type);
					if (CupCakeCooker.CanCook())
						CupCakeCooker.Cook();
				}
				break;
			}

			case IngredientType.Chocolate:
			case IngredientType.Mint:
			{
				if (MintIceCreamCooker.Add(type))
				{
					RemoveItemFromPlayer(type);
					if (MintIceCreamCooker.CanCook())
						MintIceCreamCooker.Cook();
				}
				break;
			}
		}
	}

	private void RemoveItemFromPlayer(IngredientType item)
	{
		Player.Inventory[item]--;
		InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	public void SelectCooker(GameObject go)
	{
		//Debug.Log("Selected " + go.name);
		//SelectedCooker = go.GetComponent<Cooker>();
	}

	void UpdateCookerAvailability()
	{
		//foreach (var c in Cookers)
		//{
		//	//var canUse = c.Recipe.Satisfied(Player.Inventory);
		//	//if (!canUse)
		//	//	c.
		//}
	}

	public void BackToShop()
	{
		World.ChangeArea(0);
	}

	void OnDisable()
	{
		//Debug.Log("BakeryArea enabled: " + gameObject.activeSelf);
	}

	public void RemoveIngredient(GameObject go)
	{
	}
}

