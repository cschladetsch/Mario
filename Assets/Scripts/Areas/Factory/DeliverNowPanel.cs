using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeliverNowPanel : MarioObject
{
	public Button YesButton;
	public Button NoButton;
	public Text DisplayText;

	private DeliveryTruck _truck;

	public float DeliverNowCostFraction = 0.4f;

	public void UpdateDisplayTex()
	{
		DisplayText.text = string.Format("Deliver now for {0} gold?", CalcDeliveryCost());
	}

	/// <summary>
	/// How much it would cost to immediately deliver the truck`
	/// </summary>
	/// <returns></returns>
	public int CalcDeliveryCost()
	{
		var sum = 0;
		foreach (var kv in _truck._contents)
		{
			var item = kv.Key;
			var count = kv.Value;
			sum += World.GetInfo(item).Buy*count;
		}

		var fullCost = sum*DeliverNowCostFraction;
		var percent = _truck.ProgressBar.PercentFinished;
		return Mathf.Max(2, (int)(percent*fullCost));
	}

	public void DeliverNowCancelled()
	{
		Debug.Log("DeliverNow Cancelled");
		gameObject.SetActive(false);

	}
	public void DeliverNowPressed()
	{
		var deliveryCost = CalcDeliveryCost();

		Debug.Log("DeliverNow Pressed: Gold: " + Player.Gold + ", cost: " + deliveryCost);

		if (Player.Gold < deliveryCost)
		{
			Debug.Log("Not enough gold to deliver now");
			return;
		}

		Debug.Log("Delivering now...");
		Player.RemoveGold(deliveryCost);
		gameObject.SetActive(false);
		_truck.Complete();
	}	

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
		_truck = FindObjectOfType<DeliveryTruck>();
	}

	protected override void Tick()
	{
		YesButton.interactable = Player.Gold >= CalcDeliveryCost();
	}
}
