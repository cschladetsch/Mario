using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductsPanelScript : MarioObject
{
	public List<ProductItem> Products = new List<ProductItem>();

	public ProgressBar SellProgressBar;

	protected override void Construct()
	{
		foreach (var go in gameObject.GetComponentsInChildren<ProductItem>())
		{
			Debug.Log("Have product " + go.Type);
			Products.Add(go);
		}
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
}
