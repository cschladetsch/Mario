﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flow;
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
	public Dictionary<IngredientType, int> _contents = new Dictionary<IngredientType, int>();

	public bool HasAnything
	{
		get { return _contents.Sum(c => c.Value) > 0; }
	}

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
		Debug.Log("BuyingAreaUI.ResetContents");
		_contents = IngredientItem.CreateIngredientDict<int>();
	}

	protected override void Tick()
	{
		base.Tick();

		UpdateDisplay();
	}

	public void TimerButttonPressed()
	{
		var truck = FindObjectOfType<DeliveryTruck>();
		//Debug.Log("TimerButtonPressed: ready=" + truck.Ready);

		if (!truck.Ready)
		{
			if (_contents.Sum(c => c.Value) == 0)
			{
				Debug.Log("Nothing to deliver!");
				return;
			}

			truck.Deliver(_contents);
			ClearButtons();
			ClearContents();
		}
		else
			truck.CompleteDeliveryToFactory();
	}

	private void ClearButtons()
	{
		foreach (var b in _buttons)
			b.SetAmount(0);
	}

	private void UpdateCosts()
	{
	}

	/// <summary>
	/// CompleteDeliveryToFactory the order, remove the UI, start the truck
	/// </summary>
	public void FinishOrder()
	{
		var truck = FindObjectOfType<DeliveryTruck>();
		truck.Deliver(_contents);
	}

	public void EnterGame()
	{
		var truck = FindObjectOfType<DeliveryTruck>();
		truck.CompleteDeliveryToFactory();
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
		if (_contents.Sum(c => c.Value) == 6)
		{
			Debug.Log("Currently limited to 6 items max");
			return;
		}

		var button = go.GetComponent<IngredientButtton>();
		var type = button.Type;
		var info = World.IngredientInfo[type];
		if (Player.Gold < info.Buy)
			return;

		button.AddAmount(1);
		if (!_contents.ContainsKey(type))
		{
			Debug.LogError("Shop doesn't have entry for " + type);
			return;
		}

		_contents[type]++;
		Player.Gold -= info.Buy;
		UpdateDisplay();
	}

	public void SellItem(GameObject go)
	{
		var type = go.GetComponent<IngredientItem>().Type;
		if (_contents[type] == 0) // && Player.Inventory[item] == 0)
			return;

		var gold = World.IngredientInfo[type].Sell;
		Player.Gold += gold;
		_contents[type]--;

		Kernel.Factory.NewCoroutine(MoveSoldItem, type);
		UpdateDisplay();
	}

	private IEnumerator MoveSoldItem(IGenerator self, IngredientType sold)
	{
		yield break;
	}

	/// <summary>
	/// Start a bake
	/// </summary>
	public void BakePressed()
	{
		World.ChangeArea(AreaType.Bakery);
	}

	private void OnDisable()
	{
		//Debug.Log(" " + name + " was disabled");
	}

	//public void Reset()
	//{
	//	//ResetContents();

	//	foreach (var b in _buttons)
	//	{
	//		b.Amount = 0;
	//		b.UpdateUi();
	//	}

	//	InventoryPanel.UpdateDisplay(Player.Inventory, false);
	//}
	public void ClearContents()
	{
		//Debug.Log("BuyingArea.ClearContents");
		_contents = IngredientItem.CreateIngredientDict<int>();
		UpdateDisplay();
	}
}