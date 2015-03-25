using UnityEngine;

public class Bomb : Pickup
{
	public float Radius = 1;

	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Debug.Log("Boom!");

		Player.BombHit(this);

		Remove();

		return false;
	}
}