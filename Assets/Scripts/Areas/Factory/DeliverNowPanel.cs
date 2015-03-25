using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Controller for the panel that pops up when the player
/// taps on the delivery van while it is delivering
/// </summary>
public class DeliverNowPanel : MarioObject
{
	public Button YesButton;

	public Button NoButton;

	public Text DisplayText;

	public float DeliverNowCostFraction = 0.4f;

	private DeliveryTruck _truck;

	protected override void Begin()
	{
		base.Begin();
		_truck = FindObjectOfType<DeliveryTruck>();
	}

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

		//// TODO WTF why does this keep happening
		// ANSWER: if the game object starts disabled in the scene, it's Awake and Start methods
		// are NOT called(!)
		//if (World == null)
		//	World = FindObjectOfType<World>();

		foreach (var kv in _truck.Contents)
		{
			var item = kv.Key;
			var count = kv.Value;
			sum += World.GetInfo(item).Buy*count;
		}

		var fullCost = sum*DeliverNowCostFraction;
		var percent = (1.0f - _truck.ProgressBar.PercentFinished)*3;	// 0 -> 2, 1 -> 0
		return Mathf.Max(2, (int) (percent*fullCost));
	}

	private void OnDestroy()
	{
		//Debug.Log("DeliverNowPanel.Destroy");
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
		_truck.CompleteDeliveryToFactory();
	}

	protected override void Tick()
	{
		YesButton.interactable = Player.Gold >= CalcDeliveryCost();
	}
}