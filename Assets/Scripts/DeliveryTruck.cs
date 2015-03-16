using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryTruck : MarioObject
{
	public GameObject BuyingOptions;

	public GameObject PlayButton;

	public Text PlayButtonText;

	private List<IngredientButtton> _buttons = new List<IngredientButtton>();

	private void UpdateDisplay()
	{
		// disable factory button if we have no contents
		//World.Buttons.FactoryButton.interactable = _contents.Sum(c => c.Value) > 0;

		Canvas.UpdateGoldAmount();

		foreach (var c in _buttons)
			c.UpdateUi();
	}

	public void VanButtonPressed()
	{
		if (_delivering)
			return;

		BuyingOptions.SetActive(true);
	}

	public void PlayButtonPressed()
	{
		Complete();
	}

	public void OrderButtonPressed()
	{
		BuyingOptions.SetActive(false);
		PlayButton.SetActive(true);
		Deliver(_contents);
	}

	/// <summary>
	/// True if truck has been delivered
	/// </summary>
	public bool Ready;

	/// <summary>
	/// How long it takes to deliver
	/// </summary>
	public float DeliveryTime;

	public void BuyItem(GameObject go)
	{
		if (_contents.Sum(c => c.Value) == 6)
		{
			Debug.Log("Currently limited to 6 items max");
			return;
		}

		var button = go.GetComponent<IngredientButtton>();
		var type = button.Type;
		var info = World.IngredientInfo[type];
		if (Player.Gold < info.Buy)
			return;

		button.AddAmount(1);
		if (!_contents.ContainsKey(type))
		{
			Debug.LogError("Shop doesn't have entry for " + type);
			return;
		}

		_contents[type]++;
		Player.Gold -= info.Buy;
		UpdateDisplay();
	}

	/// <summary>
	/// Collider for car
	/// </summary>
	public Collider2D Collider;

	public Button Button;

	public Button TimerButtton;

	private float _deliveryTimer;

	private bool _delivering;

	private Dictionary<IngredientType, int> _contents;

	protected override void Begin()
	{
		base.Begin();

		BuyingOptions.SetActive(false);
		PlayButton.SetActive(false);

		_buttons = BuyingOptions.transform.GetComponentsInChildren<IngredientButtton>().ToList();
		Debug.Log("Found " + _buttons.Count + " ingredients buttons");
		if (_buttons.Count == 0)
		{
			foreach (Transform tr in BuyingOptions.transform)
			{
				var component = tr.GetComponent<IngredientButtton>();
				Debug.Log("BuyingOption " + tr.name + ", comp: " + component);
				if (component != null)
					_buttons.Add(component);
			}
		}

		_contents = IngredientItem.CreateIngredientDict<int>();

		UpdateDisplay();
	}

	public void Deliver(Dictionary<IngredientType, int> contents)
	{
		if (TestForSkip(contents))
			return;

		_deliveryTimer = DeliveryTime;
		_delivering = true;

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

	protected override void Tick()
	{
		//var canDeliver = World.BuyingAreaUi.HasAnything;

		//TimerButtton.interactable = canDeliver;
		//Button.interactable = canDeliver;

		base.Tick();

		UpdateTimer();

		UpdateDelivering();

		//UpdatePressed();
	}

	private void UpdatePressed()
	{
		Debug.Log("UpdatePressed: ready=" + Ready);
		if (!Ready)
			return;

		var mouse = Input.GetMouseButtonDown(0);
		Debug.Log("mouse: " + mouse);
		if (!mouse)
			return;

		var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		Debug.Log("collider: " + hit.collider);
		if (hit.collider == null)
			return;

		Complete();
	}

	public void Complete()
	{
		Debug.LogWarning("DeliveryTruck.Complete");

		if (!Ready)
			return;

		TurnTimerOn(false);

		//if (World.CurrentLevel == null)
			World.ChangeArea(AreaType.Factory);

		World.CurrentLevel.AddIngredients(_contents);

		Ready = false;
		_contents.Clear();
		_delivering = false;
		_deliveryTimer = DeliveryTime;

		//World.BuyingAreaUi.ClearContents();

		ResetTruck();
	}

	private void TurnTimerOn(bool on)
	{
		//Canvas.CarTimer.transform.parent.gameObject.SetActive(on);
	}

	private void UpdateDelivering()
	{
		if (!_delivering)
			return;

		_deliveryTimer -= RealDeltaTime;
		Ready = _deliveryTimer <= 0;
		if (!Ready)
			return;

		_delivering = false;
		PlayButtonText.text = "Ready";
		PlayButtonText.color = Color.green;
	}

	private void UpdateTimer()
	{
		if (!_delivering)
			return;

		var text = string.Format("{0:0.0}s", _deliveryTimer);
		PlayButtonText.text = text;
		PlayButtonText.color = Color.black;
	}

	private void ResetTruck()
	{
		//Debug.Log("DeliveryCar.ResetTruck");
		TurnTimerOn(true);
		Ready = false;
		_delivering = false;

		// MON
		//Canvas.CarTimer.text = string.Format("{0:0.0}s", DeliveryTime);
	}
}
