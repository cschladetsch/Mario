using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngredientButtton : MarioObject
{
	public IngredientType Type;

	public Text CostText;

	public Text AmountText;

	public Image Image;

	public int Amount;

	public void Reset()
	{
		base.BeforeFirstUpdate();

		if (!World.IngredientInfo.ContainsKey(Type))
		{
			Debug.LogWarning("World doesn't know about ingredient: " + Type);
			return;
		}

		var info = World.IngredientInfo[Type];
		var tex = info.Image;
		Image.overrideSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(1, 1));

		CostText.text = string.Format("{0}$", info.Buy);
		AmountText.text = "0";
	}

	public void AddAmount(int n)
	{
		Amount = int.Parse(AmountText.text) + n;
		UpdateUi();
	}

	public void UpdateUi()
	{
		AmountText.text = Amount.ToString();
	}

	public void SetAmount(int i)
	{
		Amount = i;
		AmountText.text = i.ToString();
	}
}
