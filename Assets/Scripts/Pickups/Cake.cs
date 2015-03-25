using System.Collections;
using Flow;
using UnityEngine;

/// <summary>
/// The visual representation of a cake in the game.
/// </summary>
public class Cake : Pickup
{
	/// <summary>
	/// Where we will end up in the truck, as we are being delivered to it
	/// </summary>
	internal ParabolaUI TruckParabola;

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

		if (!gameObject.activeSelf)
			return;

		if (next)
		{
			World.Kernel.Factory.NewCoroutine(TransitionCake, conv, next);
			return;
		}

		// no next conveyor, add to truck
		Truck.AddCake(this);
	}

	public float TransitionTime = 3;

	private IEnumerator TransitionCake(IGenerator self, Conveyor from, Conveyor to)
	{
		var start = transform.position;
		var end = to.GetStartLocation();
		var mid = start + (end - start)/2.0f;

		var disp = 1.5f;
		if (from.MoveRight)
			mid.x += disp;
		else
			mid.x -= disp;

		var para = new ParabolaUI(start, mid, end, TransitionTime);
		Debug.DrawLine(start, mid, Color.green, 5);
		Debug.DrawLine(mid, end, Color.red, 5);

		while (!para.Completed)
		{
			transform.position = para.UpdatePos();
			yield return 0;
		}

		to.AddItem(this, 0);

		yield break;
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
			Debug.Log("Cake '" + Type + "' has fallen for too long, destroying");
			Player.DroppedCake(this);
			CurrentLevel.DestroyCake(this);
			if (CurrentLevel.NoMoreCakes)
				Truck.StartEmptying();
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