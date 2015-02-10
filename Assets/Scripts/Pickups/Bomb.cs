using UnityEngine;

public class Bomb : Pickup
{
	public float Radius = 1;

	public override void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		base.CharacterHit(character, conv, next);

		var player = FindObjectOfType<Player>();

		player.BombHit(this);

		Remove();

		Debug.Log("Boom!");
	}

	protected override void StartDropped(bool moveRight)
	{
		Remove();
	}
}

