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

	private DeliveryTruck _truck;

	protected override void Begin()
	{
		base.Begin();
		_truck = FindObjectOfType<DeliveryTruck>();
	}

	public void UpdateDisplayTex()
	{
		DisplayText.text = string.Format("Deliver for {0}$?", _truck.CalcDeliveryCost());
	}

	/// <summary>
	/// How much it would cost to immediately deliver the truck`
	/// </summary>
	/// <returns></returns>
	

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
		var deliveryCost = _truck.CalcDeliveryCost();

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
		YesButton.interactable = Player.Gold >= _truck.CalcDeliveryCost();
	}
}