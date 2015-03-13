using System.Collections.Generic;
using UnityEngine;

public class Cake : Pickup
{
	/// <summary>
	/// Where we will end up in the truck
	/// </summary>
	internal Parabola TruckParabola;

	/// <summary>
	/// The related information about this ingredient is in World.Ingredients[Type]
	/// </summary>
	public IngredientType Type;

	/// <summary>
	/// The UI label to update with the cost amount
	/// </summary>
	public UnityEngine.UI.Text CostText;

	/// <summary>
	/// If true, this item has been delivered and is resting on the Truck
	/// </summary>
	public bool Delivered;

	public void Drop()
	{
		StartDropped(false);
	}

	public override void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		base.CharacterHit(character, conv, next);

		conv.RemoveItem(this);

		if (next)
		{
			next.AddItem(this, 0);
			return;
		}

		var truck = FindObjectOfType<Truck>();
		if (truck.Emptying)
		{
			Debug.Log("Dropped cake because truck is still emptying");
			Drop();
			return;
		}

		truck.AddCake(this);
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

	void OnDestroy()
	{
		Debug.Log("Cake destroyed " + Time.frameCount);
	}
}
