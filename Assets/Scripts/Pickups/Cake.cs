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

	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		if (!base.CharacterHit(character, conv, next))
		{
			return false;
		}

		if (!gameObject)
			return false;

		if (!gameObject.activeSelf)
			return false;

		if (Cake.Is(Type))
			conv.RemoveItem(this);

		if (next)
		{
			World.Kernel.Factory.NewCoroutine(TransitionCake, conv, next);
			return true;
		}

		// no next conveyor, add to truck
		Truck.AddCake(this);

		return true;
	}

	public float TransitionTime = 3;

	private IEnumerator TransitionCake(IGenerator self, Conveyor from, Conveyor to)
	{
		// we got destroyed when we got picked up at end of conveyor
		if (!gameObject)
			yield break;

		if (from == World.CurrentLevel.GetConveyor(0))
		{
			// TODO: animate straight across
			to.AddItem(this, 0);
			yield break;
		}

		var radius = 1.0f;
		var time = 0.5f;
		var startAngle = Mathf.Deg2Rad*270.0f;
		var endAngle = Mathf.Deg2Rad*90.0f;
		var da = endAngle - startAngle;

		var dir = from.MoveRight ? -1.0f : 1.0f;
		var startPos = transform.position;
		for (var t = 0.0f; t < time; t += Time.deltaTime)
		{
			var angle = startAngle + dir*t/time*da;

			var x = radius*(Mathf.Cos(angle));
			var y = radius*(Mathf.Sin(angle));
			transform.position = startPos + new Vector3(x,y  + radius,0);
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

	public static bool Is(IngredientType type)
	{
		return type != IngredientType.Bomb && type != IngredientType.ExtraLife;
	}
}