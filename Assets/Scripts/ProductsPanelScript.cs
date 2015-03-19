using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductsPanelScript : MarioObject
{
	public List<ProductItem> Products = new List<ProductItem>();

	public ProgressBar SellProgressBar;

	protected override void Construct()
	{
		Products.AddRange(gameObject.GetComponentsInChildren<ProductItem>());
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
		foreach (var p in Products)
		{
			p.UpdateUi();
		}
	}

	public GameObject GetProduct(IngredientType type)
	{
		var prod = Products.FirstOrDefault(p => p.Type == type);
		return prod == null ? null : prod.gameObject;
	}
}