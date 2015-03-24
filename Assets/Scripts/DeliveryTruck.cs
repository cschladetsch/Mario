using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The van that delivers goods to the factory
/// </summary>
public class DeliveryTruck : MarioObject
{
	public DeliverNowPanel DeliverNowPanel;

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

	internal Dictionary<IngredientType, int> Contents;

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
		{
			ShowDeliverNowPanel();
			return;
		}

		GatherIngredientButtons();
		BuyingOptions.SetActive(true);
	}

	private void ShowDeliverNowPanel()
	{
		DeliverNowPanel.UpdateDisplayTex();
		DeliverNowPanel.gameObject.SetActive(true);
	}

	public void PlayButtonPressed()
	{
		Complete();
	}

	/// <summary>
	/// Player wishes to start delivering items to factory
	/// </summary>
	public void OrderButtonPressed()
	{
		Debug.Log("Delivery time for " + Contents.Sum(c => c.Value) + " is " + CalcDeliveryTime());

		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);
		ProgressBar.gameObject.SetActive(true);
		ProgressBar.Reset();
		ProgressBar.TotalTime = CalcDeliveryTime();
		ProgressBar.Paused = false;
		Deliver(Contents);
	}

	public void BuyItem(GameObject go)
	{
		if (Contents.Sum(c => c.Value) == 6)
		{
			//Debug.Log("Currently limited to 6 items max");
			return;
		}

		var button = go.GetComponent<IngredientButtton>();
		var type = button.Type;

		var info = World.IngredientInfo[type];
		if (Player.Gold < info.Buy)
			return;

		AddAnimation(type, go);
		button.AddAmount(1);

		Contents[type]++;
		Player.Gold -= info.Buy;
		UpdateDisplay();
	}

	private void AddAnimation(IngredientType type, GameObject dest)
	{
		var end = dest;
		ItemAnimation.Animate(type, Canvas.PlayerGold.gameObject, end, 2);
	}

	protected override void Begin()
	{
		base.Begin();

		DeliverNowPanel.gameObject.SetActive(false);
		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);
		ProgressBar.gameObject.SetActive(false);

		GatherIngredientButtons();

		Contents = IngredientItem.CreateIngredientDict<int>();

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
			if (ing == null) 
				continue;

			_buttons.Add(ing);
			ing.SetAmount(0);
			ing.SetCost(World.IngredientInfo[ing.Type].Buy);
		}
	}

	public void Deliver(Dictionary<IngredientType, int> contents)
	{
		if (TestForSkip(contents))
			return;

		_deliveryTimer = CalcDeliveryTime();
		_delivering = true;

		PlayButton.gameObject.SetActive(false);

		Contents = new Dictionary<IngredientType, int>();
		foreach (var kv in contents)
			Contents.Add(kv.Key, kv.Value);

		Ready = false;
	}

	private float CalcDeliveryTime()
	{
		return DeliveryTime + Contents.Sum(c => c.Value);
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
		Contents = IngredientItem.CreateIngredientDict<int>();
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

		UpdateDelivering();
	}

	private void UpdateDeliveryButton()
	{
		DeliveryButton.interactable = Contents.Sum(c => c.Value) > 0;
	}

	public void Complete()
	{
		Debug.LogWarning("DeliveryTruck.Complete");

		PlayButton.SetActive(true);
		ProgressBar.gameObject.SetActive(false);

		TurnTimerOn(false);

		World.ChangeArea(AreaType.Factory);
		World.CurrentLevel.AddIngredients(Contents);

		Ready = false;

		Contents = IngredientItem.CreateIngredientDict<int>();

		_delivering = false;
		_deliveryTimer = CalcDeliveryTime();

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

	private void ResetTruck()
	{
		//Debug.Log("DeliveryCar.ResetTruck");
		TurnTimerOn(true);
		Ready = false;
		_delivering = false;
		PlayButton.SetActive(false);
	}

	public void ShowBuyingPanel(bool show)
	{
		BuyingOptions.SetActive(show);
	}

	public void BuyItem(IngredientItem item)
	{
		foreach (var b in _buttons.Where(b => b.Type == item.Type))
		{
			BuyItem(b.gameObject);
		}
	}

	public bool HasItems(IngredientType type, int num)
	{
		return _buttons.Where(b => b.Type == type).Any(b => b.Amount >= num);
	}

	public void Deliver()
	{
		Deliver(Contents);
	}

	public void Reset()
	{
		Contents = IngredientItem.CreateIngredientDict<int>();
		_delivering = false;
	}
}
