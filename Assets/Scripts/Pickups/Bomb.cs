using UnityEngine;

public class Bomb : Cake
{
	public float Radius = 1;

	public override bool CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
		Debug.Log("Boom!");

		Player.BombHit(this);

		Remove();

		return false;
	}

	private bool _hack;

	/// <summary>
	/// TODO: put into base class for all items that progress through conveyors without being
	/// dropped, like extra-lives, bombs, and wrong ingredients
	/// </summary>
	/// <param name="moveRight"></param>
	protected override void StartDropped(bool moveRight)
	{
		// TODO: why is this called twice in same frame? this is a temp. work-around
		if (_hack)
			return;
		_hack = true;

		UnityEngine.Debug.Log("Extra Life StartDrop");
		var conv = Conveyor;
		var next = CurrentLevel.GetNextConveyor(conv);

		World.Kernel.Factory.NewCoroutine(TransitionCake, conv, next).Completed += f =>
		{
			_hack = false;
			Reset();
		};
	}
}