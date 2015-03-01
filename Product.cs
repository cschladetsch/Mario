using System.Collections.Generic;

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

	public Dictionary<Type, Recipe> Recipies; 

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


