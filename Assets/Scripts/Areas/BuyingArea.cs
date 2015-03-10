using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Flow;
using UnityEngine;

/// <summary>
/// Areas where people by things that were made in the bakery
/// </summary>
public class BuyingArea : AreaBase
{
	public List<Cake> Ingredients = new List<Cake>();

	/// <summary>
	/// Default delivery truck wait time
	/// </summary>
	public float BaseTruckWaitTime = 5;

	/// <summary>
	/// How to make a delivery car
	/// </summary>
	public GameObject DeliveryTruckPrefab;

	/// <summary>
	/// Distance from XX/y plane of delivery car
	/// </summary>
	public float DeliverryCarDepth = 2;

	public GameObject DeliveryTruck;

	/// <summary>
	/// How long it takes the delivery truck to complete a delivery
	/// </summary>
	public float DeliveryTruckTime = 30;

	/// <summary>
	/// Where the delivery truck starts
	/// </summary>
	public float StartX = -15;

	/// <summary>
	/// Where the delivery truck ends
	/// </summary>
	public float EndX = 9;

	/// <summary>
	/// how the delivery truck is as it moves along the scene
	/// </summary>
	public float DeliveryTruckHeight = -1.6f;

	protected override void Begin()
	{
		base.Begin();

		UiCanvas.SetActive(true);

		World.Canvas.GoalPanel.GetComponent<GoalPanel>().UpdateUi();
	}

	private IEnumerator TapToContinue(IGenerator t0)
	{
		Debug.Log("Start tap to continue");
		World.BeginArea(2);
		yield break;
	}

	protected override void BeforeFirstUpdate()
	{
		//base.BeforeFirstUpdate();
		//var truck = Kernel.Factory.NewPeriodicTimer(TimeSpan.FromSeconds(BaseTruckWaitTime));
		//truck.Elapsed += NewTruck;
	}

	private void NewTruck(ITransient sender)
	{
		Debug.Log("Truck Arrived!");
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public override void Next()
	{
		base.Next();
	}

	public bool Skip;

	public void StartDeliveryTruck(Dictionary<IngredientType, int> contents)
	{
		if (Skip)
		{
			foreach (var c in contents)
			{
				Player.Ingredients[c.Key] += c.Value;
			}
			World.BeginArea(3);
			return;
		}

		// remove items already owned by player
		foreach (var i in Player.Ingredients)
			contents[i.Key] -= i.Value;

		var go = (GameObject) Instantiate(DeliveryTruckPrefab);
		var truck = go.GetComponent<DeliveryTruck>();
		truck.Deliver(StartX, EndX, DeliveryTruckTime, DeliveryTruckHeight, DeliverryCarDepth, contents);
	}
}


