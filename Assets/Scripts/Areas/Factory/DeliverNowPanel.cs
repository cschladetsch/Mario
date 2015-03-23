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
		DisplayText.text = string.Format("Deliver for {0}$?", CalcDeliveryCost());
	}

	/// <summary>
	/// How much it would cost to immediately deliver the truck`
	/// </summary>
	/// <returns></returns>
	public int CalcDeliveryCost()
	{
		var sum = 0;
		if (_truck == null)
			_truck = FindObjectOfType<DeliveryTruck>();

		// TODO WTF why does this keep happening
		if (World == null)
			World = FindObjectOfType<World>();

		foreach (var kv in _truck.Contents)
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
		//Debug.Log("DeliverNow Cancelled");
		gameObject.SetActive(false);

	}
	public void DeliverNowPressed()
	{
		var deliveryCost = CalcDeliveryCost();

		//Debug.Log("DeliverNow Pressed: Gold: " + Player.Gold + ", cost: " + deliveryCost);

		if (Player.Gold < deliveryCost)
		{
			Debug.Log("Not enough gold to deliver now");
			return;
		}

		//Debug.Log("Delivering now...");
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
		//Debug.Log("DeliverNowPanel.Begin: " + _truck + " world=" + World);
	}

	protected override void Tick()
	{
		YesButton.interactable = Player.Gold >= CalcDeliveryCost();
	}
}
