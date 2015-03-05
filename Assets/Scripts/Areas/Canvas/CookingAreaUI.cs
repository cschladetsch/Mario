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

		foreach (var c in Cookers)
			c.Select(false);
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
		}
	}

	public void IngredientButtonPressed(GameObject button)
	{
		Debug.Log("Pressed " + button.name);
		if (!SelectedCooker)
			return;
		var item = button.GetComponent<IngredientItem>().Type;
		SelectedCooker.IngredientButtonPressed(item);
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

