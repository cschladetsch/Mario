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

	public void SetAmount(int amount, bool avail)
	{
		if (Count == null)
		{
			Debug.LogWarning("Ingredient button for " + Type + " is null?" + transform.parent.name);
			return;
		}

		//Debug.Log("Setting type " + Type + " : avail " + avail + " num " + amount);
		Count.text = amount.ToString();
		Count.color = avail ? Color.green : Color.red;
	}
}