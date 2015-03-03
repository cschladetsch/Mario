using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class SellingAreaCanvas : MarioObject
{
	/// <summary>
	/// The visuals for the current inventory in the canvas
	/// </summary>
	public InventoryPanel InventoryPanel;

	/// <summary>
	/// The current items that will be purchased. these can be bought and sold many times before
	/// the player presses the 'Done' button and the truck starts its delivery
	/// </summary>
	readonly Dictionary<Ingredient.TypeEnum, int> _contents = new Dictionary<Ingredient.TypeEnum, int>();

	public UnityEngine.UI.Text GoldText;

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		UpdateDisplay();

		GatherIngredients();

		GatherCostTexts();

		UpdateCosts();
	}

	private void GatherIngredients()
	{
		foreach (var e in Enum.GetValues(typeof(Ingredient.TypeEnum)))
			_contents.Add((Ingredient.TypeEnum)e, 0);	
	}

	private void GatherCostTexts()
	{
		foreach (var ingredient in GetIngredientPanels())
		{
			ingredient.CostText = ingredient.transform.FindChild("Cost").GetComponent<UnityEngine.UI.Text>();
		}
	}

	private void UpdateCosts()
	{
		foreach (var ingredient in GetIngredientPanels())
		{
			ingredient.UpdateCostText();
		}
	}

	// ReSharper disable once ReturnTypeCanBeEnumerable.Local
	private Ingredient[] GetIngredientPanels()
	{
		return transform.GetComponentsInChildren<Ingredient>();
	}

	/// <summary>
	/// Complete the order, remove the UI, start the truck
	/// </summary>
	public void FinishOrder()
	{
		Debug.Log("Finish Order");
		var area = World.CurrentArea as SellingArea;
		World.CurrentArea.UiCanvas.gameObject.SetActive(false);
		area.StartDeliveryTruck(_contents);
	}

	/// <summary>
	/// Order an in ingredient - or remove one. This is awkward because Unity 4.6 doesn't
	/// seem to allow for multiple arguments to OnCLick events any more
	/// </summary>
	/// <param name="button"></param>
	public void OrderIngredient(GameObject button)
	{
		var ing = button.transform.parent.GetComponent<Ingredient>();
		var type = ing.Type;
		var cost = ing.BaseCost;
		var amount = int.Parse(button.GetComponent<UnityEngine.UI.Button>().name);
		var totalCost = cost*amount;
		var gold = Player.Gold;

		// can't afford it
		if (!Player.GodMode && totalCost > gold)
			return;

		// can't go negative
		if (totalCost > 0 && totalCost + gold < 0)
			return;

		var stock = _contents[type];

		var nextAmount = stock + amount;
		if (nextAmount < 0)
			return;

		_contents[type] = nextAmount;

		Debug.Log(string.Format("Currnet {0}, totalCost {1}, new {2}", gold, totalCost, gold - totalCost));

		Player.Gold -= totalCost;

		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		UpdateGoldDisplay();
		InventoryPanel.UpdateDisplay(_contents);
	}

	private void UpdateGoldDisplay()
	{
		//Debug.Log("Player " + Player);
		if (!Player)
			return;

		GoldText.text = Player.Gold.ToString();
	}

	void OnDisable()
	{
		Debug.Log(" " + name + " was disabled");
	}
}

