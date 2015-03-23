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

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();

		SellingProductsPanel = FindObjectOfType<ProductsPanelScript>();
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
	}

	public override void SellItem(IngredientType type)
	{
		base.SellItem(type);

		var move = Kernel.Factory.NewCoroutine(MoveSoldItem, type);
		var sell = Kernel.Factory.NewCoroutine(ItemSold, type);
		sell.ResumeAfter(move);
	}

	IEnumerator ItemSold(IGenerator self, IngredientType type)
	{
		//Debug.Log("SOLD a " + type);

		SellingProductsPanel.SellProgressBar.Reset();

		ItemSold(type);

		yield break;
	}

	public float SoldItemTravelTime = 2;

	IEnumerator MoveSoldItem(IGenerator self, IngredientType type)
	{
		Player.RemoveItem(type);

		// make the image to move
		var go = (GameObject)Instantiate(World.GetInfo(type).ImagePrefab);
		go.transform.SetParent(World.Canvas.transform);

		var product = SellingProductsPanel.GetProduct(type);

		var start = product.transform.position;
		var end = World.Canvas.GoldPanel.gameObject.transform.position;
		var mid = start + (end - start)/2.0f;
		mid.x += UnityEngine.Random.Range(-Screen.width/10, Screen.width/10);
		mid.y += UnityEngine.Random.Range(-Screen.height/10, Screen.height/10);
		var para = new ParabolaUI(start, mid, end, SoldItemTravelTime);

		while (true)
		{
			if (para.Completed)
			{
				Destroy(go);
				yield break;
			}

			var pos = para.UpdatePos();
			go.transform.position = new Vector3(pos.x, pos.y, 0);

			yield return 0;
		}
	}

	public override void EnterArea()
	{
		base.EnterArea();

		//Debug.Log("Cooking area begins");

		var ui = FindObjectOfType<CookingAreaUI>();
		ui.InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

}