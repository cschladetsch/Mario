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

	public float DeliverNowCostFraction = 0.4f;

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

	public GameObject BuyingOptions;

	public GameObject PlayButton;

	public Text PlayButtonText;

	public Text DeliverText;

	public ProgressBar ProgressBar;

	private readonly List<IngredientButtton> _buttons = new List<IngredientButtton>();

	public float _deliveryTimer;

	private bool _delivering;

	internal Dictionary<IngredientType, int> Contents;

	private MakeGlow _glow;

	public bool Delivering
	{
		get { return _delivering; }
	}

	public bool Pulling;

	private void UpdateDisplay()
	{
		Canvas.UpdateGoldAmount();

		foreach (var c in _buttons)
			c.UpdateUi();
	}

	public void VanButtonPressed()
	{
		if (_glow.enabled)
		{
			_glow.enabled = false;
			GetComponent<Image>().color = Color.white;
		}

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
		CompleteDeliveryToFactory();
	}

	/// <summary>
	/// Player wishes to start delivering items to factory
	/// </summary>
	public void OrderButtonPressed()
	{
		ProgressBar.Reset();

		//Debug.Log("Delivery time for " + Contents.Sum(c => c.Value) + " is " + CalcDeliveryTime());
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
		if (Pulling)
			return;

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
		_glow = GetComponent<MakeGlow>();

		base.Begin();

		DeliverNowPanel.gameObject.SetActive(false);
		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);
		ProgressBar.gameObject.SetActive(false);

		GatherIngredientButtons();

		Contents = IngredientItem.CreateIngredientDict<int>();

		UpdateDisplay();

		ProgressBar.TotalTime = CalcDeliveryTime();
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
		//if (World.GodMode)
		//	return 1;

		//return DeliveryTime + Contents.Sum(c => c.Value);
		return DeliveryTime;
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
		RefundItems();
		Contents = IngredientItem.CreateIngredientDict<int>();
		UpdateDisplay();
	}

	private void RefundItems()
	{
		Debug.Log("RefundItems: " + _buttons.Count);
		if (_buttons.Count == 0)
			GatherIngredientButtons();

		foreach (var b in _buttons)
		{
			if (b.Amount == 0)
				continue;

			Debug.Log("Refunding " + b.Type + "x" + b.Amount + " for " + World.GetInfo(b.Type).Buy + " each");
			Player.Gold += b.Amount*World.GetInfo(b.Type).Buy;

			b.SetAmount(0);
		}
	}

	protected override void Tick()
	{
		base.Tick();

		DeliverText.text = string.Format("{0}$ to Deliver", (int)CalcDeliveryCost());

		UpdateDeliveryButton();

		UpdateDelivering();
	}

	private void UpdateDeliveryButton()
	{
		DeliveryButton.interactable = Contents.Sum(c => c.Value) > 0;
	}

	public int CalcDeliveryCost()
	{
		var sum = 0;
		foreach (var kv in Contents)
		{
			var item = kv.Key;
			var count = kv.Value;
			sum += World.GetInfo(item).Buy*count;
		}

		var fullCost = sum*DeliverNowCostFraction;
		var percent = (1.0f - ProgressBar.PercentFinished)*3;	// 0 -> 2, 1 -> 0
		return Mathf.Max(2, (int)(percent*fullCost));
	}

	public void CompleteDeliveryToFactory()
	{
		var sum = Contents.Sum(c => c.Value);
		//Debug.LogWarning("DeliveryTruck.CompleteDeliveryToFactory: " + sum);
		if (sum == 0)
		{
			Debug.LogError("No delivery truck contents??");
			Reset();
		}

		PlayButton.SetActive(true);
		ProgressBar.gameObject.SetActive(false);

		TurnTimerOn(false);

		var cp = Contents.ToDictionary(kv => kv.Key, kv => kv.Value);
		World.ChangeArea(AreaType.Factory);
		World.CurrentLevel.AddIngredients(cp);

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
		ProgressBar.Reset();
		Deliver(Contents);
	}

	public void Reset()
	{
		//Debug.Log("DeliveryTruck.Reset");
		Contents = IngredientItem.CreateIngredientDict<int>();
		_delivering = false;
		Pulling = false;
		ProgressBar.Reset();
	}
}