using System;
using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

/// <summary>
/// Controller for the area in which the player creates products from other
/// ingredients via Cookers
/// </summary>
public class BakeryArea : AreaBase
{
	/// <summary>
	/// What can currently be made
	/// </summary>
	public List<Recipe> Recipes = new List<Recipe>();

	public ProductsPanelScript SellingProductsPanel;

	public DeliveryTruck DeliveryTruck;

	public InventoryPanel InventoryPanel;

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();

		SellingProductsPanel = FindObjectOfType<ProductsPanelScript>();
		DeliveryTruck = FindObjectOfType<DeliveryTruck>();
		InventoryPanel = FindObjectOfType<InventoryPanel>();
	}

	public override void Reset()
	{
		base.Reset();
		DeliveryTruck.Reset();
		InventoryPanel.Reset();
		SellingProductsPanel.Reset();
	}

	public void IngredientButtonPressed(IngredientType type)
	{
		Debug.Log("Added a " + type);
		switch (type)
		{
			case IngredientType.Cherry:
				break;
		}
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();

#if DEBUG
		if (Input.GetKeyDown(KeyCode.C))
			DeliveryTruck.CompleteDeliveryToFactory();
#endif
	}

	public float SoldItemTravelTime = 2;

	public override void SellItem(IngredientType type)
	{
		Debug.Log("BakeryArea.SellItem: " + Time.frameCount);
		base.SellItem(type);

		var move = ItemAnimation.Animate(type, SellingProductsPanel.GetProduct(type), Canvas.PlayerGold.gameObject, SoldItemTravelTime);
		var sell = Kernel.Factory.NewCoroutine(ItemSold, type);
		sell.ResumeAfter(move);
	}

	IEnumerator ItemSold(IGenerator self, IngredientType type)
	{
		//SellingProductsPanel.GetButton(type).Reset();

		ItemSold(type);

		yield break;
	}

	public override void EnterArea()
	{
		base.EnterArea();

		//Debug.Log("Cooking area begins");

		var ui = FindObjectOfType<CookingAreaUI>();
		ui.InventoryPanel.UpdateDisplay(Player.Inventory, false);

		DeliveryTruck = FindObjectOfType<DeliveryTruck>();

		DeliveryTruck.Reset();
		DeliveryTruck.ShowBuyingPanel(false);
	}

	/// <summary>
	/// Move all items from truck to inventory panel then reset it
	/// </summary>
	/// <param name="cakes"></param>
	/// <returns></returns>
	public IGenerator TakeDelivery(List<Cake> cakes)
	{
		DeliveryTruck.Pulling = true;
		var take = Kernel.Factory.NewCoroutine(TakeDelivery, cakes);
		take.Completed += f => DeliveryTruck.Reset();
		return take;
	}

	public IEnumerator TakeDelivery(IGenerator self, List<Cake> cakes)
	{
		Debug.Log("BakeryArea.TakeDelivery: " + cakes.Count);

		foreach (var c in cakes)
		{
			ItemAnimation.Animate(c.Type, DeliveryTruck.gameObject,
				InventoryPanel.GetButton(c.Type).gameObject, 2, AddIngredient);

			yield return self.ResumeAfter(TimeSpan.FromSeconds(UnityEngine.Random.Range(1, 2)));
		}

		DeliveryTruck.Reset();

		self.Complete();
	}

	private void AddIngredient(IngredientType type)
	{
		Player.AddCake(type);
	}
}