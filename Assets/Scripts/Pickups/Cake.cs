using UnityEngine;

/// <summary>
/// The visual representation of a cake in the game.
/// </summary>
public class Cake : Pickup
{
	/// <summary>
	/// Where we will end up in the truck, as we are being delivered to it
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

	/// <summary>
	/// The cake has dropped from edge of top-right conveyor, probably because
	/// the truck is currently emptying
	/// </summary>
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

		// no next conveyor, add to truck
		Truck.AddCake(this);
	}

	protected override void Tick()
	{
		base.Tick();

		if (!Dropped)
		{
			//Debug.Log(Dropped);
			return;
		}

		_droppedTimer -= GameDeltaTime;
		if (_droppedTimer < 0)
		{
			CurrentLevel.DestroyCake(this);
		}
	}

	/// <summary>
	/// The item has started to drop from end of conveyor
	/// </summary>
	/// <param name="moveRight"></param>
	protected override void StartDropped(bool moveRight)
	{
		base.StartDropped(moveRight);

		_droppedTimer = 2;
		_dropped = true;

		rigidbody2D.isKinematic = false;
		const float push = 120;
		var force = new Vector2(moveRight ? push : -push, -20);
		rigidbody2D.AddForce(force);
	}

	/// <summary>
	/// Write the cost to the UI
	/// </summary>
	public void UpdateCostText()
	{
		var text = World.IngredientInfo[Type].Sell.ToString();
		CostText.text = string.Format("{0}$", text);
	}

#if DEBUG
	private void OnDestroy()
	{
		//Debug.Log("Cake destroyed " + Time.frameCount);
	}
#endif
}