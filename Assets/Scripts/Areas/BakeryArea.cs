using System;
using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEditorInternal.VersionControl;
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
			DeliveryTruck.Complete();
#endif
	}

	public float SoldItemTravelTime = 2;

	public override void SellItem(IngredientType type)
	{
		base.SellItem(type);

		var move = ItemAnimation.Animate(type, SellingProductsPanel.GetProduct(type), Canvas.GoldPanel, SoldItemTravelTime);
		var sell = Kernel.Factory.NewCoroutine(ItemSold, type);
		sell.ResumeAfter(move);
	}

	IEnumerator ItemSold(IGenerator self, IngredientType type)
	{
		SellingProductsPanel.SellProgressBar.Reset();

		ItemSold(type);

		yield break;
	}

	public override void EnterArea()
	{
		base.EnterArea();

		//Debug.Log("Cooking area begins");

		var ui = FindObjectOfType<CookingAreaUI>();
		ui.InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	public IGenerator TakeDelivery(List<Cake> cakes)
	{
		return Kernel.Factory.NewCoroutine(TakeDelivery, cakes);
	}

	public IEnumerator TakeDelivery(IGenerator self, List<Cake> cakes)
	{
		foreach (var c in cakes)
		{
			ItemAnimation.Animate(c.Type, DeliveryTruck.gameObject,
				InventoryPanel.GetButton(c.Type).gameObject, 2, AddIngredient);

			yield return self.ResumeAfter(TimeSpan.FromSeconds(UnityEngine.Random.Range(1, 2)));
		}
	}

	private void AddIngredient(IngredientType type)
	{
		Player.AddCake(type);
	}
}