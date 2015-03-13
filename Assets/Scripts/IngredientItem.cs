using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientItem : MonoBehaviour
{
	public IngredientType Type;

	public static Dictionary<IngredientType, T> CreateIngredientDict<T>()
	{
		var ing = new Dictionary<IngredientType, T>();
		foreach (var e in Enum.GetValues(typeof(IngredientType)))
		{
			var type = (IngredientType)e;
			if (type != IngredientType.None)
				ing.Add(type, default(T));
		}
		return ing;
	}
}
