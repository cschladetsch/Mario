using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientItem : MonoBehaviour
{
	public IngredientType Type;

	public static Dictionary<IngredientType, T> CreateIngredientDict<T>()
	{
		var ing = new Dictionary<IngredientType, T>();
		foreach (var e in Enum.GetValues(typeof (IngredientType)))
			ing.Add((IngredientType) e, default(T));
		return ing;
	}
}
