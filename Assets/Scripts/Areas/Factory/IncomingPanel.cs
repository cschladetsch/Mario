using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class IncomingPanel : MarioObject
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

	public void Populate()
	{
		Debug.Log("IncomingPanel.Popuplate: " + World.CurrentLevel.Inventory.Count);
	}

	public Vector3 DroppedCakePosition(IngredientType type)
	{
		Debug.Log("DroppedCakePosition");
		//foreach (var go in transform.GetComponentsInChildren<>()
	}
}
