using UnityEngine;
using UnityEngine.UI;

public class IngredientButtton : MarioObject
{
	public IngredientType Type;

	public PriceText PriceText;

	public Text AmountText;

	public Image Image;

	public int Amount;

	private Button _button;

	public int MinGoalIndex;

	protected override void Begin()
	{
		base.Begin();

		_button = GetComponent<Button>();
	}

	protected override void Tick()
	{
		base.Tick();

		_button.interactable = World.GoalIndex >= MinGoalIndex;
	}

	public void Reset()
	{
		base.BeforeFirstUpdate();

		if (!World.IngredientInfo.ContainsKey(Type))
		{
			Debug.LogWarning("World doesn't know about ingredient: " + Type);
			return;
		}

		var info = World.GetIngredientInfo(Type);
		var go = (GameObject) Instantiate(info.ImagePrefab);
		Image = go.GetComponent<Image>();

		SetAmount(info.Buy);
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

	public void SetCost(int buy)
	{
		PriceText.SetAmount(buy);
	}
}