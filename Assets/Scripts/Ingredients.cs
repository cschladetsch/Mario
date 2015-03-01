using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Ingredient : MarioObject
{
	public enum Type
	{
		Cherry,
		Muffin,
		Chocolate,
		Raisen,
		Strawberry,
	}

	private Dictionary<List<Ingredient.Type>, Product> Produucts = new Dictionary<List<Ingredient.Type>, Product>();

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


