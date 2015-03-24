using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductItem : MarioObject
{
	public ProgressBar ProgressBar;

	public Text Amount;

	public Text Price;

	public IngredientType Type;
	private IngredientInfo _ingredientInfo;

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
		if (Price == null)
			return;

		var shadow = Price.transform.FindChild("Shadow").GetComponent<Text>();
		_ingredientInfo = World.GetInfo(Type);
		shadow.text = Price.text = _ingredientInfo.Sell.ToString();

		ProgressBar.TotalTime = _ingredientInfo.SellingTime;
		ProgressBar.Reset();
		ProgressBar.Ended -= ProgressBarEnded;
		ProgressBar.Ended += ProgressBarEnded;
	}

	private void ProgressBarEnded(ProgressBar pb)
	{
		var elapsed = pb.Elapsed;
		while (elapsed > _ingredientInfo.SellingTime)
		{
			if (Player.Inventory[Type] == 0)
				break;

			//Debug.Log("SOLD from ProductItem");
			World.CurrentArea.SellItem(Type);

			elapsed -= _ingredientInfo.SellingTime;
		}

		ProgressBar.Reset();
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