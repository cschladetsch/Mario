using System.Diagnostics;

/// <summary>
/// A Pickup that adds an extra life to the player
/// </summary>
public class ExtraLife : Cake
{
	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Player.AddLife();

		Remove();

		//UnityEngine.Debug.Log("Character hit extra life, returning false");

		return false;
	}

	//protected override void StartDropped(bool moveRight)
	//{
	//	base.StartDropped(moveRight);

	//	Remove();
	//}
}