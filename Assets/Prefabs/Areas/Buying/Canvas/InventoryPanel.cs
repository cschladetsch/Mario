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

	public void UpdateDisplay(Dictionary<Ingredient.TypeEnum, int> contents)
	{
		if (contents.Count == 0)
			return;

		CherryCount.text = contents[Ingredient.TypeEnum.Cherry].ToString();
		MuffinCount.text = contents[Ingredient.TypeEnum.Muffin].ToString();
		CupcakeCount.text = contents[Ingredient.TypeEnum.CupCake].ToString();
	}
}


