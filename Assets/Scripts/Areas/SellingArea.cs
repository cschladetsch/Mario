using System;
using Flow;
using UnityEngine;

/// <summary>
/// Areas where people by things that were made in the bakery
/// </summary>
public class SellingArea : AreaBase
{
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

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
		var truck = Kernel.Factory.NewPeriodicTimer(TimeSpan.FromSeconds(BaseTruckWaitTime));
		truck.Elapsed += NewTruck;
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
		World.NextArea();
	}
}


