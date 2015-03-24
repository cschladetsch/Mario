using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductsPanelScript : MarioObject
{
	public List<ProductItem> Products = new List<ProductItem>();

	//public ProgressBar SellProgressBar;

	//public void RemoveProduct(IngredientType type)
	//{
	//	foreach (var prod in Products.Where(prod => prod.Type == type))
	//	{
	//		prod.UpdateUi();
	//		return;
	//	}
	//}

	protected override void Construct()
	{
		GatherProducts();
	}

	private void GatherProducts()
	{
		Products.AddRange(gameObject.GetComponentsInChildren<ProductItem>());
		//Debug.Log("ProductsPanelScript: " + Products.Count);
		//foreach (var p in Products)
		//	Debug.Log(p.Type);
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
		//foreach (var p in Products)
		//{
		//	p.UpdateUi();
		//}
	}

	public GameObject GetProduct(IngredientType type)
	{
		//Debug.Log("ProductPanelScript.GetProduct: " + type + ", prods.Count=" + Products.Count);
		//foreach (var p in Products)
		//	Debug.Log(p.Type);

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