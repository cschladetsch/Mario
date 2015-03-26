using UnityEngine;

public class Bomb : AutoMoveItemBase
{
	public float Radius = 1;

	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Player.BombHit(this);

		Remove();

		// TODO: remove all other items within the Radius

		return false;
	}
}