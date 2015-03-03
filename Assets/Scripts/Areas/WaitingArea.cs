using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingArea : AreaBase
{
	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public override void Next()
	{
		Debug.Log("Next PRessed on waiting");

		base.Next();

		//World.NextLevel();
	}
}


