using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverPanel : MarioObject
{
	public void Pressed()
	{
		gameObject.SetActive(false);
		World.Reset();
		World.ChangeArea(AreaType.Bakery);
	}

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
	}
}