using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductItem : MarioObject
{
	public ProgressBar ProgressBar;

	public Text Amount;

	public Text Price;

	public IngredientType Type;

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
		if (Price == null)
			return;

		var shadow = Price.transform.FindChild("Shadow").GetComponent<Text>();
		shadow.text = Price.text = World.GetInfo(Type).Sell.ToString();

		ProgressBar.TotalTime = World.GetInfo(Type).SellingTime;
		ProgressBar.Reset();
		ProgressBar.Ended += ProgressBarEnded;
	}

	private void ProgressBarEnded(ProgressBar pb)
	{
		ProgressBar.Reset();

		if (Player.Inventory[Type] == 0)
			return;

		World.CurrentArea.SellItem(Type);
	}

	protected override void Tick()
	{
		var num = Player.Inventory[Type];
		//Debug.Log(string.Format("Paused {0}, num {1}", ProgressBar.Paused, num));
		if (ProgressBar.Paused && num > 0)
		{
			ProgressBar.Reset();
			ProgressBar.Paused = false;
		}
	}

	public void UpdateUi()
	{
		Amount.text = Player.Inventory[Type].ToString();
	}
}