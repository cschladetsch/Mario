﻿using System;
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

	public UnityEngine.UI.Button ShopButton;
 
	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		GatherCookers();

		UpdateDisplay();
		//foreach (var c in Cookers)
		//	c.(false);
	}

	public void AddIngredient(GameObject ing)
	{
		var cooker = ing.transform.parent.GetComponent<Cooker>();
		var type = ing.GetComponent<IngredientItem>().Type;

		if (Player.Ingredients[type] > 0)
		{
			if (cooker.Add(type))
			{
				Player.Ingredients[type]--;
				if (cooker.CanCook())
					cooker.Cook();
				UpdateDisplay();
			}
		}
	}

	private void UpdateDisplay()
	{
		InventoryPanel.UpdateDisplay(Player.Ingredients, false);
	}

	private void GatherCookers()
	{
		foreach (Transform tr in transform)
		{
			var cooker = tr.GetComponent<Cooker>();
			if (!cooker)
				continue;

			Cookers.Add(cooker);
			//Debug.Log("Found a cooker for " + cooker.Recipe.name);

			//  HACKS
			if (cooker.name == "CupcakeCooker")
				_cakes = cooker;
			else
				_iceCream = cooker;
		}
	}

	protected override void Tick()
	{
		base.Tick();

		//var cooking = !_cakes.Cooking() && !_iceCream.Cooking();
		//ShopButton.interactable = !cooking;
	}

	private Cooker _cakes;
	private Cooker _iceCream;

	public void IngredientButtonPressed(GameObject button)
	{
		//Debug.Log("Pressed " + button.name);
		//if (!SelectedCooker)
		//	return;
		var type = button.GetComponent<IngredientItem>().Type;
		if (Player.Ingredients[type] == 0)
			return;

		//SelectedCooker.IngredientButtonPressed(item);

		// HACKS
		switch (type)
		{
			case IngredientType.Cherry:
			case IngredientType.Muffin:
			{
				if (_cakes.Add(type))
				{
					RemoveItemFromPlayer(type);
					if (_cakes.CanCook())
						_cakes.Cook();
				}
				break;
			}

			case IngredientType.Chocolate:
			case IngredientType.Mint:
			{
				if (_iceCream.Add(type))
				{
					RemoveItemFromPlayer(type);
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
		InventoryPanel.UpdateDisplay(Player.Ingredients, false);
	}

	public void SelectCooker(GameObject go)
	{
		Debug.Log("Selected " + go.name);
		SelectedCooker = go.GetComponent<Cooker>();
	}

	void UpdateCookerAvailability()
	{
		//foreach (var c in Cookers)
		//{
		//	//var canUse = c.Recipe.Satisfied(Player.Ingredients);
		//	//if (!canUse)
		//	//	c.
		//}
	}

	public void BackToShop()
	{
		World.BeginArea(0);
	}

	void OnDisable()
	{
		//Debug.Log("CookingArea enabled: " + gameObject.activeSelf);
	}

	public void RemoveIngredient(GameObject go)
	{
	}
}
