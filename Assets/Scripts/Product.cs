using System;
using System.Collections.Generic;
using UnityEngine;

public class Product : MarioObject
{
	public enum Type
	{
		PlainMuffin,
		ChocolateMuffin,
		CherryMuffin,
		ChocolateCake,
		CheeryCake,
		CheeseCake,
	}

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
}


