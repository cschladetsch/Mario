using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class BuyingAreaUI : MarioObject
{
	/// <summary>
	/// The visuals for the current inventory in the canvas
	/// </summary>
	public InventoryPanel InventoryPanel;

	public Toggle SkipToggle;

	/// <summary>
	/// The current items that will be purchased. these can be bought and sold many times before
	/// the player presses the 'Done' button and the truck starts its delivery
	/// </summary>
	Dictionary<IngredientType, int> _contents = new Dictionary<IngredientType, int>();

	public UnityEngine.UI.Text GoldText;

	private List<IngredientButtton> _buttons = new List<IngredientButtton>(); 

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		ResetContents();

		UpdateCosts();

		GatherButtons();

		UpdateDisplay();
	}

	private void GatherButtons()
	{
		_buttons = transform.FindChild("BuyingOptions").GetComponentsInChildren<IngredientButtton>().ToList();
	}

	private void ResetContents()
	{
		_contents = IngredientItem.CreateIngredientDict<int>();
	}

	protected override void Tick()
	{
		base.Tick();

		//UpdateDisplay();
	}

	private void UpdateCosts()
	{
	}

	/// <summary>
	/// Complete the order, remove the UI, start the truck
	/// </summary>
	public void FinishOrder()
	{
		var truck = FindObjectOfType<DeliveryTruck>();
		truck.Deliver(_contents);
	}

	public void EnterGame()
	{
		var truck = FindObjectOfType<DeliveryTruck>();
		truck.Complete();
	}

	private void UpdateDisplay()
	{
		// disable factory button if we have no contents
		//World.Buttons.FactoryButton.interactable = _contents.Sum(c => c.Value) > 0;

		Canvas.UpdateGoldAmount();

		//InventoryPanel.UpdateDisplay(_contents, false);
		InventoryPanel.UpdateDisplay(Player.Inventory, true);

		foreach (var c in _buttons)
			c.UpdateUi();
	}

	public void BuyItem(GameObject go)
	{
		if (_contents.Count == 6)
		{
			Debug.Log("Currently limited to 6 items max");
			return;
		}

		var button = go.GetComponent<IngredientButtton>();
		var item = button.Type;
		var info = World.IngredientInfo[item];
		if (Player.Gold < info.Buy)
			return;

		button.AddAmount(1);
		_contents[item]++;
		Player.Gold -= info.Buy;
		UpdateDisplay();
	}

	public void SellItem(GameObject go)
	{
		var item = go.GetComponent<IngredientItem>().Type;
		if (_contents[item] == 0)// && Player.Inventory[item] == 0)
			return;

		var gold = World.IngredientInfo[item].Sell;
		Player.Gold += gold;
		_contents[item]--;

		UpdateDisplay();
	}

	/// <summary>
	/// Start a bake
	/// </summary>
	public void BakePressed()
	{
		World.BeginArea(AreaType.Bakery);
	}

	void OnDisable()
	{
		//Debug.Log(" " + name + " was disabled");
	}

	public void Reset()
	{
		ResetContents();

		foreach (var b in _buttons)
		{
			b.Amount = 0;
			b.UpdateUi();
		}
	}
}
