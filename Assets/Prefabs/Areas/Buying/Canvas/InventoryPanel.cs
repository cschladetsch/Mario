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
			{
				Debug.Log("child item " + c.name + " has no IngredientItem");
				continue;
			}

			var count = item.gameObject.transform.FindChild("Count");
			if (count != null)
				Counts[item.Type] = count.GetComponent<UnityEngine.UI.Text>();
		}

		Counts.Remove(IngredientType.None);

		//foreach (var c in Counts)
		//{
		//	Debug.Log(string.Format("{0} = {1}", c.Key, c.Value.name));
		//}
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public void UpdateDisplay(Dictionary<IngredientType, int> contents, bool add)
	{
		Debug.Log("InventoryPanel.UpdateDisplay");

		if (Counts == null)
			CreateDict();

		foreach (var ing in contents)
		{
			// HACK: why do this
			if (ing.Key == IngredientType.None)
				return;

			if (!Counts.ContainsKey(ing.Key))
				continue;

			var num = ing.Value;
			if (add)
			{
				num += int.Parse(Counts[ing.Key].text);
			}

			Counts[ing.Key].text = num.ToString();
		}
	}

}
