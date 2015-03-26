using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class AutoMoveItemBase : Cake
{
	private bool _hack;

	protected override void StartDropped(bool moveRight)
	{
		// TODO: why is this called twice in same frame? this is a temp. work-around
		if (_hack)
			return;
		_hack = true;

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
