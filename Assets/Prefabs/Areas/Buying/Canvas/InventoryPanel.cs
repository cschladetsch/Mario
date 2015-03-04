using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MarioObject
{
	public UnityEngine.UI.Text CherryCount;
	public UnityEngine.UI.Text MuffinCount;
	public UnityEngine.UI.Text CupcakeCount;

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public void UpdateDisplay(Dictionary<IngredientType, int> contents)
	{
		if (contents.Count == 0)
			return;

		CherryCount.text = contents[IngredientType.Cherry].ToString();
		MuffinCount.text = contents[IngredientType.Muffin].ToString();
		CupcakeCount.text = contents[IngredientType.CupCake].ToString();
	}
}


