using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using Flow;
using UnityEngine;

/// <summary>
/// Areas where people by things that were made in the bakery
/// </summary>
public class SellingArea : AreaBase
{
	public List<Ingredient> Ingredients = new List<Ingredient>();

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
	public float DeliveryTruckHeight = -5;

	protected override void Begin()
	{
		base.Begin();

		DeliveryTruck = (GameObject) Instantiate(DeliveryTruckPrefab);
		DeliveryTruck.transform.SetZ(DeliverryCarDepth);
				
		var move = Kernel.Factory.NewCoroutine(MoveDeliveryVan, StartX, EndX);
		Kernel.Factory.NewCoroutine(TapToContinue).ResumeAfter(move);
	}

	IEnumerator MoveDeliveryVan(IGenerator self, float start, float end)
	{
		var speed = (end - start)/DeliveryTruckTime;
		while (DeliveryTruck.transform.position.x < end)
		{
			var delta = DeltaTime*speed;
			DeliveryTruck.transform.SetX(DeliveryTruck.transform.position.x + delta);
			yield return 0;
		}
	}

	private IEnumerator TapToContinue(IGenerator t0)
	{
		Debug.Log("Start tap to continue");
		yield break;
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
	}

	public void FinishOrder()
	{
		Debug.Log("Finish Order");
		World.NextArea();
	}

	public void OrderIngredient(int val)
	{
		var p = transform.parent;
		Debug.Log(p.name);
		Debug.Log(p.GetComponent<Ingredient>());

		var ing = transform.GetComponentInParent<Ingredient>();
		Debug.Log(ing.name + " " + val);
	}
}


