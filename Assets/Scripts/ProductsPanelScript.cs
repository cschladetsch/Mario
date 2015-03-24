using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is basically the Shop for the game - products get produced by cookers and 
/// stored here and sold indpenendantly by the products buttons as children in scene
/// </summary>
public class ProductsPanelScript : MarioObject
{
	/// <summary>
	/// The products that can be sold
	/// </summary>
	public List<ProductItem> Products = new List<ProductItem>();

	protected override void Construct()
	{
		GatherProducts();
	}

	private void GatherProducts()
	{
		Products.AddRange(gameObject.GetComponentsInChildren<ProductItem>());
	}

	public GameObject GetProduct(IngredientType type)
	{
		var prod = Products.FirstOrDefault(p => p.Type == type);
		return prod == null ? null : prod.gameObject;
	}

	public void UpdateUi()
	{
		//Debug.Log("ProductsPanelScript.UpdateUI");
		foreach (var p in Products)
			p.UpdateUi();
	}

	public void Reset()
	{
		GatherProducts();
		UpdateUi();
	}
}
