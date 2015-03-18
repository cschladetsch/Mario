using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for the area in which the player creates prodducts from other
/// ingredients via Cookers
/// </summary>
public class BakeryArea : AreaBase
{
	/// <summary>
	/// What can currently be made
	/// </summary>
	public List<Recipe> Recipes = new List<Recipe>();

	public void IngredientButtonPressed(IngredientType type)
	{
		Debug.Log("Added a " + type);
		switch (type)
		{
			case IngredientType.Cherry:
				break;
		}
	}

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	public void IngredientButtonPressed(GameObject button)
	{
		//Debug.Log("Pressed " + button.name);
		//var item = button.GetComponent<IngredientItem>().Type;
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public override void EnterArea()
	{
		base.EnterArea();

		//Debug.Log("Cooking area begins");

		var ui = FindObjectOfType<CookingAreaUI>();
		ui.InventoryPanel.UpdateDisplay(Player.Inventory, false);

		FindObjectOfType<DeliveryTruck>().ShowBuyingPanel(false);
	}
}
