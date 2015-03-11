using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryTruck : MarioObject
{
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

	public Button TimerButtton;

	private float _deliveryTimer;

	private bool _delivering;

	private Dictionary<IngredientType, int> _contents;

	public void Deliver(Dictionary<IngredientType, int> contents)
	{
		if (TestForSkip(contents)) 
			return;

		_deliveryTimer = DeliveryTime;
		_delivering = true;
		_contents = contents;
		Ready = false;
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
		if (!World.BuyingAreaUi.SkipToggle || !World.BuyingAreaUi.SkipToggle.isOn) 
			return false;

		foreach (var c in contents)
			Player.Inventory[c.Key] += c.Value;

		World.BeginArea(AreaType.Bakery);

		return true;
	}

	protected override void Tick()
	{
		var canDeliver = World.BuyingAreaUi.HasAnything;

		TimerButtton.interactable = canDeliver;
		Button.interactable = canDeliver;

		base.Tick();

		UpdateTimer();

		UpdateDelivering();

		UpdatePressed();
	}

	private void UpdatePressed()
	{
		if (!Ready)
			return;

		if (!Input.GetMouseButtonDown(0)) 
			return;

		var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider == null) 
			return;

		Complete();
	}

	public void Complete()
	{
		if (!Ready)
			return;

		TurnTimerOn(false);
		World.BeginMainGame(_contents);
	}

	private void TurnTimerOn(bool on)
	{
		Canvas.CarTimer.transform.parent.gameObject.SetActive(on);
	}

	private void UpdateDelivering()
	{
		if (!_delivering) 
			return;

		_deliveryTimer -= DeltaTime;
		Ready = _deliveryTimer <= 0;
		if (!Ready) 
			return;

		_delivering = false;
		var text = Canvas.CarTimer;
		text.text = "Ready";
		text.color = Color.green;
	}

	private void UpdateTimer()
	{
		if (!_delivering)
			return;

		var text = string.Format("{0:0.0}s", _deliveryTimer);
		Canvas.CarTimer.text = text;
		Canvas.CarTimer.color = Color.black;
	}

	public void Reset()
	{
		//Debug.Log("DeliveryCar.Reset");
		TurnTimerOn(true);
		Ready = false;
		_delivering = false;
		Canvas.CarTimer.text = "Deliver";
	}
}
