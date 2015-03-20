using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic qualities of an ingredient; mostly used to map IngredientType to Image,
/// but also used in Cookers
/// </summary>
public class IngredientItem : MarioObject
{
	public IngredientType Type;

	public Text Count;

	public bool IsCookingIngredient;

	private DeliveryTruck _truck;

	protected override void Begin()
	{
		base.Begin();

		//Count = transform.FindChild("Count").GetComponent<Text>();

		_truck = FindObjectOfType<DeliveryTruck>();
	}

	public static Dictionary<IngredientType, T> CreateIngredientDict<T>()
	{
		var ing = new Dictionary<IngredientType, T>();
		foreach (var e in Enum.GetValues(typeof (IngredientType)))
		{
			var type = (IngredientType) e;
			if (type != IngredientType.None)
				ing.Add(type, default(T));
		}
		return ing;
	}

	protected override void Tick()
	{
		base.Tick();

		//UpdateUi();
	}

	public void UpdateUi()
	{
		if (!Count || !IsCookingIngredient)
			return;

		//var req = int.Parse(Count.text);
		//var hasEnough = req <= Player.Inventory[Type];
		//var color = hasEnough ? Color.green : Color.red;
		//if (!hasEnough && _truck.HasItems(Type, req))
		//	color = Color.yellow;
		//Count.color = color;
	}

	public void SetAmount(int amount, bool avail)
	{
		if (Count == null)
		{
			Debug.LogWarning("Ingredient button for " + Type + " is null?" + transform.parent.name);
			return;
		}

		//Debug.Log("Setting type " + Type + " : avail " + avail);

		Count.text = amount.ToString();
		Count.color = avail ? Color.green : Color.red;
	}
}