using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductItem : MarioObject
{
	public Text Amount;

	public IngredientType Type;

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
	}

	public void UpdateUi()
	{
		Amount.text = Player.Inventory[Type].ToString();
	}
}
