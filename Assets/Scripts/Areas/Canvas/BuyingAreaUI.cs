using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class BuyingAreaUI : MarioObject
{
	/// <summary>
	/// The visuals for the current inventory in the canvas
	/// </summary>
	public InventoryPanel InventoryPanel;

	/// <summary>
	/// The current items that will be purchased. these can be bought and sold many times before
	/// the player presses the 'Done' button and the truck starts its delivery
	/// </summary>
	readonly Dictionary<IngredientType, int> _contents = new Dictionary<IngredientType, int>();

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
		foreach (var e in Enum.GetValues(typeof(IngredientType)))
			_contents.Add((IngredientType)e, 0);	
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
	private Cake[] GetIngredientPanels()
	{
		return transform.GetComponentsInChildren<Cake>();
	}

	/// <summary>
	/// Complete the order, remove the UI, start the truck
	/// </summary>
	public void FinishOrder()
	{
		//Debug.Log("Finish Order");
		var area = World.CurrentArea as BuyingArea;
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
		var ing = button.transform.parent.GetComponent<Cake>();
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

		Player.Gold -= totalCost;

		if (totalCost > 0)
			Player.Ingredients[type]++;
		else
			Player.Ingredients[type]--;

		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		var any = _contents.Sum(c => c.Value) > 0;
		Canvas.OrderButton.interactable = any;

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

	/// <summary>
	/// Start a bake
	/// </summary>
	public void BakePressed()
	{
		var area = World.CurrentArea as BuyingArea;
		World.CurrentArea.UiCanvas.gameObject.SetActive(false);
		World.BeginArea(3);
	}

	void OnDisable()
	{
		//Debug.Log(" " + name + " was disabled");
	}
}

