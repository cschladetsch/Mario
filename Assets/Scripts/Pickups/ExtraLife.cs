using UnityEngine;

/// <summary>
/// A Pickup that adds an extra life to the player
/// </summary>
public class ExtraLife : AutoMoveItemBase
{
	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Player.AddLife();

		conv.RemoveItem(this);

		// TODO: doesn't work because character is in scene-space and LivesRemainingText is in canvas
		//ItemAnimation.Animate(IngredientType.ExtraLife, character.gameObject, Player.LivesRemainingText.gameObject, 1);

		Remove();

		return false;
	}

}