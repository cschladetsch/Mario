using UnityEngine;

/// <summary>
/// A Pickup that adds an extra life to the player
/// </summary>
public class ExtraLife : Cake
{
	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Player.AddLife();

		conv.RemoveItem(this);

		Debug.Log("Char: " + character.name);

		ItemAnimation.Animate(IngredientType.ExtraLife, character.gameObject, Player.LivesRemainingText.gameObject, 1);

		Remove();

		return false;
	}

	private bool _hack;

	protected override void StartDropped(bool moveRight)
	{
		// TODO: why is this called twice in same frame? this is a temp. work-around
		if (_hack)
			return;
		_hack = true;

		//UnityEngine.Debug.Log("Extra Life StartDrop");
		var conv = Conveyor;
		var next = CurrentLevel.GetNextConveyor(conv);

		conv.RemoveItem(this);

		World.Kernel.Factory.NewCoroutine(TransitionCake, conv, next).Completed += f =>
		{
			_hack = false;
			Reset();
		};
	}
}