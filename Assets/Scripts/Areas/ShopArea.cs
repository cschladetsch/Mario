using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

/// <summary>
/// Areas where people by things that were made in the bakery
/// </summary>
public class ShopArea : AreaBase
{
	/// <summary>
	/// What is to be delivered
	/// </summary>
	public List<Cake> Ingredients = new List<Cake>();

	public bool Skip;

	///// <summary>
	///// Default delivery truck wait time
	///// </summary>
	//public float BaseTruckWaitTime = 5;

	public DeliveryTruck DeliveryTruck;

	/// <summary>
	/// How long it takes the delivery truck to complete a delivery
	/// </summary>
	public float DeliveryTruckTime = 30;

	protected override void Begin()
	{
		base.Begin();

		UiCanvas.SetActive(true);

		World.Canvas.GoalPanel.GetComponent<GoalPanel>().UpdateUi();
	}

	private IEnumerator TapToContinue(IGenerator t0)
	{
		Debug.Log("Start tap to continue");
		//World.ChangeArea(AreaType.Shop);
		yield break;
	}

	protected override void BeforeFirstUpdate()
	{
		DeliveryTruck = FindObjectOfType<DeliveryTruck>();
		//DeliveryTruck.Reset();

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

	public override void EnterArea()
	{
		base.EnterArea();

		Player.ShowCharacters(false);

		// TODO: why is thus.World sometimes null???
		//var world = FindObjectOfType<World>();
		//world.BuyingAreaUi.Reset();
	}

	public void StartDeliveryTruck(Dictionary<IngredientType, int> contents)
	{
		//// deliver all items immediately, then go to cooker
		//if (Skip || World.BuyingAreaUi.SkipToggle.isOn)
		//{
		//	foreach (var c in contents)
		//		Player.Inventory[c.Key] += c.Value;

		//	World.ChangeArea(AreaType.Bakery);

		//	return;
		//}

		//DeliveryTruck.Deliver(contents);
	}
}