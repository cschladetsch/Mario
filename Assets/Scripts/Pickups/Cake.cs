using System.Collections.Generic;
using UnityEngine;

public class Cake : Pickup
{
	/// <summary>
	/// Where we will end up in the truck
	/// </summary>
	internal Parabola TruckParabola;

	public IngredientType Type;

	///// <summary>
	///// How much it takes to buy one
	///// </summary>
	//public int BaseCost = 2;

	///// <summary>
	///// How much it takes to sell one
	///// </summary>
	//public int BasePrice = 1;

	/// <summary>
	/// The UI label to update with the cost amount
	/// </summary>
	public UnityEngine.UI.Text CostText;

	public override void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		base.CharacterHit(character, conv, next);

		conv.RemoveItem(this);

		if (next)
		{
			next.AddItem(this, 0);
		}
		else
		{
			var truck = FindObjectOfType<Truck>();
			//Debug.Log("Adding a " + Type + " to truck called " + name);
			truck.AddCake(this);
		}
	}

	/// <summary>
	/// The item has started to drop from end of conveyor
	/// </summary>
	/// <param name="moveRight"></param>
	protected override void StartDropped(bool moveRight)
	{
		base.StartDropped(moveRight);

		FindObjectOfType<Player>().DroppedCake(this);

		_droppedTimer = 2;
		_dropped = true;

		rigidbody2D.isKinematic = false;
		const float F = 120;
		var force = new Vector2(moveRight ? F : -F, -20);
		rigidbody2D.AddForce(force);
	}

	/// <summary>
	/// Write the cost to the UI
	/// </summary>
	public void UpdateCostText()
	{
		var world = FindObjectOfType<World>();
		var text = world.IngredientInfo[Type].Sell.ToString();
		CostText.text = string.Format("{0}$", text);
	}
}
