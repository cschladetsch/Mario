using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TestCooker : MarioObject
{
	protected override void Construct()
	{
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
	}

	public void Pressed(GameObject go)
	{
		Debug.Log("PRessed on " + go.name);
	}
}
