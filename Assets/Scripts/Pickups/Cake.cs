﻿using UnityEngine;

public class Cake : Pickup
{
	/// <summary>
	/// Where we will end up in the truck
	/// </summary>
	internal Parabola TruckParabola;

	public override void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		base.CharacterHit(character, conv, next);

		conv.RemoveItem(this);

		if (next)
			next.AddItem(this, 0);
		else
			FindObjectOfType<Truck>().AddCake(this);
	}

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
}