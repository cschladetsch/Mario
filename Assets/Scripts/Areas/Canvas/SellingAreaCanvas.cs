using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class SellingAreaCanvas : MarioObject
{
	readonly Dictionary<Ingredient.TypeEnum, int> _contents = new Dictionary<Ingredient.TypeEnum, int>();

	public UnityEngine.UI.Text GoldText;

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		UpdateGoldDisplay();

		GatherCostTexts();

		UpdateCosts();
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

		// can't afford it
		if (!Player.GodMode && cost > 0 && cost > Player.Gold)
			return;

		// can't go negative
		var totalCost = cost*amount;
		if (totalCost + Player.Gold < 0)
			return;

		Debug.Log(ing + " " +  amount);

		if (!_contents.ContainsKey(type))
			_contents.Add(type, 0);

		var stock = _contents[type];

		var nextAmount = stock + amount;
		if (nextAmount < 0)
			return;

		_contents[type] = Mathf.Max(0, nextAmount);

		Player.Gold -= totalCost;

		UpdateGoldDisplay();
	}

	private void UpdateGoldDisplay()
	{
		//Debug.Log("Player " + Player);
		GoldText.text = Player.Gold.ToString();
	}
}

