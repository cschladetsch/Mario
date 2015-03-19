/// <summary>
/// A Pickup that adds an extra life to the player
/// </summary>
public class ExtraLife : Pickup
{
	public override void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		base.CharacterHit(character, conv, next);

		Remove();

		FindObjectOfType<Player>().AddLife();
	}

	protected override void StartDropped(bool moveRight)
	{
		base.StartDropped(moveRight);

		Remove();
	}
}