using System;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MarioObject
{

	private Dictionary<IngredientType, UnityEngine.UI.Text> Counts;

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		//CreateDict();
	}

	private void CreateDict()
	{
		Counts = IngredientItem.CreateIngredientDict<UnityEngine.UI.Text>();
		for (var n = 0; n < transform.childCount; ++n)
		{
			var c = transform.GetChild(n);
			var item = c.GetComponent<IngredientItem>();
			if (item == null)
				continue;

			var count = item.gameObject.transform.FindChild("Count");
			if (count != null)
				Counts[item.Type] = count.GetComponent<UnityEngine.UI.Text>();
		}

		//foreach (var c in Counts)
		//{
		//	Debug.Log(string.Format("{0} = {1}", c.Key, c.Value.name));
		//}
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public void UpdateDisplay(Dictionary<IngredientType, int> contents)
	{
		if (Counts == null)
			CreateDict();

		//Debug.Log("===================================");

		foreach (var ing in Player.Ingredients)
		{
			// HACK: why do this
			if (ing.Key == IngredientType.None)
				return;

			if (Counts.ContainsKey(ing.Key))
			{
				//Debug.Log(Counts);
				//Debug.Log(Counts[ing.Key].name);
				//Debug.Log(ing.Value);
				Counts[ing.Key].text = ing.Value.ToString();

				//Debug.Log("Updated display for " + ing.Key + " to " + ing.Value);
			}
		}
	}
}
