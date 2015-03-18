using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The van that delivers goods to the factory
/// </summary>
public class DeliveryTruck : MarioObject
{
	public Button DeliveryButton;

	/// <summary>
	/// True if truck has been delivered
	/// </summary>
	public bool Ready;

	/// <summary>
	/// How long it takes to deliver
	/// </summary>
	public float DeliveryTime;

	/// <summary>
	/// Collider for car
	/// </summary>
	public Collider2D Collider;

	public Button Button;

	//public Button TimerButtton;

	public GameObject BuyingOptions;

	public GameObject PlayButton;

	public Text PlayButtonText;

	public ProgressBar ProgressBar;

	private readonly List<IngredientButtton> _buttons = new List<IngredientButtton>();

	public float _deliveryTimer;

	private bool _delivering;

	private Dictionary<IngredientType, int> _contents;

	public bool Delivering { get { return _delivering; } }

	private void UpdateDisplay()
	{
		Canvas.UpdateGoldAmount();

		foreach (var c in _buttons)
			c.UpdateUi();
	}

	public void VanButtonPressed()
	{
		if (Ready)
			PlayButtonPressed();

		if (_delivering)
			return;

		GatherIngredientButtons();
		BuyingOptions.SetActive(true);
	}

	public void PlayButtonPressed()
	{
		//Debug.Log("PlayButtonPressed" + "");
		Complete();
	}

	/// <summary>
	/// Player wishes to start delivering items to factory
	/// </summary>
	public void OrderButtonPressed()
	{
		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);
		ProgressBar.gameObject.SetActive(true);
		ProgressBar.Reset();
		ProgressBar.TotalTime = DeliveryTime;
		ProgressBar.Paused = false;
		Deliver(_contents);
	}

	public void BuyItem(GameObject go)
	{
		if (_contents.Sum(c => c.Value) == 6)
		{
			Debug.Log("Currently limited to 6 items max");
			return;
		}

		var button = go.GetComponent<IngredientButtton>();
		var type = button.Type;
		//if (World.GoalIndex == 0 && (type == IngredientType.Mint || type == IngredientType.Chocolate))
		//	return;

		var info = World.IngredientInfo[type];
		if (Player.Gold < info.Buy)
			return;

		button.AddAmount(1);
		//if (!_contents.ContainsKey(type))
		//{
		//	Debug.LogError("Shop doesn't have entry for " + type);
		//	return;
		//}

		_contents[type]++;
		Player.Gold -= info.Buy;
		UpdateDisplay();
	}

	protected override void Begin()
	{
		base.Begin();

		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);
		ProgressBar.gameObject.SetActive(false);

		GatherIngredientButtons();

		_contents = IngredientItem.CreateIngredientDict<int>();

		UpdateDisplay();
	}

	/// <summary>
	/// Get all ingredients from selling items panel
	/// </summary>
	private void GatherIngredientButtons()
	{
		//Debug.Log("GatherIngredientButtons: Level=" + World.GoalIndex);
		_buttons.Clear();
		foreach (Transform tr in BuyingOptions.transform)
		{
			var ing = tr.GetComponent<IngredientButtton>();
			if (ing != null)
			{
				_buttons.Add(ing);
				ing.SetAmount(0);
				ing.SetCost(World.IngredientInfo[ing.Type].Buy);

				//if (World.GoalIndex == 0 && (ing.Type == IngredientType.Mint || ing.Type == IngredientType.Chocolate))
				//	ing.gameObject.GetComponent<Button>().interactable = false;
			}
		}
	}

	public void Deliver(Dictionary<IngredientType, int> contents)
	{
		if (TestForSkip(contents))
			return;

		_deliveryTimer = DeliveryTime;
		_delivering = true;

		PlayButton.gameObject.SetActive(false);

		_contents = new Dictionary<IngredientType, int>();
		foreach (var kv in contents)
			_contents.Add(kv.Key, kv.Value);

		Ready = false;
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		ResetTruck();
	}

	/// <summary>
	/// For debugging: if the 'Skip' toggle is on in the UI, then when we 'deliver'
	/// our order, we just add the order directly to player's inventory then
	/// transition to the Bakery area
	/// </summary>
	/// <param name="contents"></param>
	/// <returns></returns>
	private bool TestForSkip(Dictionary<IngredientType, int> contents)
	{
		if (!World.CookingAreaUi.SkipToggle || !World.CookingAreaUi.SkipToggle.isOn)
			return false;

		foreach (var c in contents)
			Player.Inventory[c.Key] += c.Value;

		World.ChangeArea(AreaType.Bakery);

		return true;
	}

	public void CancelOrdering()
	{
		BuyingOptions.SetActive(false);
		_contents = IngredientItem.CreateIngredientDict<int>();
		RefundItems();
		UpdateDisplay();
	}

	private void RefundItems()
	{
		//Debug.Log("RefundItems");
		foreach (var b in _buttons)
		{
			if (b.Amount == 0)
				continue;

			//Debug.Log("Refunding " + b.Type + "x" + b.Amount + " for " + World.GetInfo(b.Type).Buy + " each");

			Player.Gold += b.Amount*World.GetInfo(b.Type).Buy;

			b.SetAmount(0);
		}
	}

	protected override void Tick()
	{
		base.Tick();

		UpdateDeliveryButton();

		UpdateTimer();

		UpdateDelivering();
	}

	private void UpdateDeliveryButton()
	{
		DeliveryButton.interactable = _contents.Sum(c => c.Value) > 0;
	}

	public void Complete()
	{
		//Debug.LogWarning("DeliveryTruck.Complete");

		if (!Ready)
			return;

		PlayButton.SetActive(true);
		ProgressBar.gameObject.SetActive(false);

		TurnTimerOn(false);

		World.ChangeArea(AreaType.Factory);
		World.CurrentLevel.AddIngredients(_contents);

		Ready = false;

		_contents = IngredientItem.CreateIngredientDict<int>();

		_delivering = false;
		_deliveryTimer = DeliveryTime;

		ResetTruck();
	}

	private void TurnTimerOn(bool on)
	{
	}

	private void UpdateDelivering()
	{
		if (!_delivering)
			return;

		_deliveryTimer -= RealDeltaTime;
		Ready = _deliveryTimer <= 0;
		if (!Ready)
			return;

		ProgressBar.gameObject.SetActive(false);
		PlayButton.SetActive(true);
	}

	private void UpdateTimer()
	{
		if (!_delivering)
			return;

		//var text = string.Format("{0:0.0}s", _deliveryTimer);
		//PlayButtonText.text = text;
		//PlayButtonText.color = Color.black;
	}

	private void ResetTruck()
	{
		//Debug.Log("DeliveryCar.ResetTruck");
		TurnTimerOn(true);
		Ready = false;
		_delivering = false;
		PlayButton.SetActive(false);
		

		// MON
		//Canvas.CarTimer.text = string.Format("{0:0.0}s", DeliveryTime);
	}

	public void ShowBuyingPanel(bool show)
	{
		BuyingOptions.SetActive(show);
	}

	public void BuyItem(IngredientItem item)
	{
		foreach (var b in _buttons)
		{
			if (b.Type == item.Type)
			{
				BuyItem(b.gameObject);
			}
		}
	}

	public bool HasItems(IngredientType type, int num)
	{
		return _buttons.Where(b => b.Type == type).Any(b => b.Amount >= num);
	}
}
