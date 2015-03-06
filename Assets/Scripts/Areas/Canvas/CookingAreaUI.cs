using System;
using System.Collections.Generic;
using System.Linq;
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

	public List<Cooker> Cookers;

	public Cooker SelectedCooker;
 
	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		GatherCookers();

		UpdateDisplay();
		//foreach (var c in Cookers)
		//	c.(false);
	}

	private void UpdateDisplay()
	{
		InventoryPanel.UpdateDisplay(Player.Ingredients);
	}

	private void GatherCookers()
	{
		foreach (Transform tr in transform)
		{
			var cooker = tr.GetComponent<Cooker>();
			if (!cooker)
				continue;

			Cookers.Add(cooker);
			Debug.Log("Found a cooker for " + cooker.Recipe.name);

			//  HACKS
			if (cooker.name == "CupcakeCooker")
				_cakes = cooker;
			else
				_iceCream = cooker;
		}

		
	}

	private Cooker _cakes, _iceCream;

	public void IngredientButtonPressed(GameObject button)
	{
		Debug.Log("Pressed " + button.name);
		//if (!SelectedCooker)
		//	return;
		var item = button.GetComponent<IngredientItem>().Type;
		//SelectedCooker.IngredientButtonPressed(item);

		// HACKS
		switch (item)
		{
			case IngredientType.Cherry:
			case IngredientType.Muffin:
			{
				if (_cakes.Add(item))
				{
					RemoveItemFromPlayer(item);
					if (_cakes.CanCook())
						_cakes.Cook();
				}
				break;
			}

			case IngredientType.Chocolate:
			case IngredientType.Mint:
			{
				if (_iceCream.Add(item))
				{
					RemoveItemFromPlayer(item);
					if (_iceCream.CanCook())
						_iceCream.Cook();
				}
				break;
			}
		}
	}

	private void RemoveItemFromPlayer(IngredientType item)
	{
		Player.Ingredients[item]--;
		InventoryPanel.UpdateDisplay(Player.Ingredients);
	}

	public void SelectCooker(GameObject go)
	{
		Debug.Log("Selected " + go.name);
		SelectedCooker = go.GetComponent<Cooker>();
	}

	void UpdateCookerAvailability()
	{
		foreach (var c in Cookers)
		{
			//var canUse = c.Recipe.Satisfied(Player.Ingredients);
			//if (!canUse)
			//	c.
		}
	}

	public void BackToShop()
	{
		World.BeginArea(0);
	}

	void OnDisable()
	{
		Debug.Log("CookingArea enabled: " + gameObject.activeSelf);
	}

	public void RemoveIngredient(GameObject go)
	{
	}
}

