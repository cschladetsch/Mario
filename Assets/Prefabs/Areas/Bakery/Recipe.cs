using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// How to make a new product from existing ingredients and other products
/// </summary>
public class Recipe : MonoBehaviour
{
	/// <summary>
	/// What is needed to make the item
	/// </summary>
	public List<IngredientType> Ingredients;

	/// <summary>
	/// The number of each corresponding ingredient required
	/// NOTE: there must be a count for each ingredient
	/// </summary>
	public List<int> Counts;

	/// <summary>
	/// How long it takes to cooks this item by default
	/// </summary>
	public float CookingTime = 30;

	/// <summary>
	/// How long it takes to cooks this item by default
	/// </summary>
	public float SellingTime = 50;

	/// <summary>
	/// The result of this recipe
	/// </summary>
	public IngredientType Result;

	public int NumResults;

	/// <summary>
	/// When this Recipe gets activated
	/// </summary>
	public int ActiveAtLevel = 0;

	/// <summary>
	/// Returns true if the given ingredient selection can be used to 
	/// make this recipe
	/// </summary>
	/// <param name="ingredients">the inputs</param>
	/// <returns>true if possible to make this recipe given the ingredients</returns>
	public bool Satisfied(Dictionary<IngredientType, int> ingredients)
	{
		for (var n = 0; n < Ingredients.Count; ++n)
		{
			var type = Ingredients[n];
			var count = Counts[n];

			if (ingredients[type] < count)
			{
				//Debug.Log("Not enough " + type + " for cooker " + Result);
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Removes items required for this Recipe from the given ingredients list
	/// </summary>
	/// <param name="ingredients"></param>
	/// <returns>true if all ingredients were removed</returns>
	public bool RemoveIngredients(Dictionary<IngredientType, int> ingredients)
	{
		if (!Satisfied(ingredients))
			return false;

		for (var n = 0; n < Ingredients.Count; ++n)
		{
			var type = Ingredients[n];
			var count = Counts[n];

			if (ingredients[type] - count < 0)
			{
				Debug.LogWarning("Couldn't remove " + type + " from ingredients list, only has " + ingredients[type]);
				return false;
			}

			ingredients[type] -= count;
		}

		return true;
	}

	public bool CanAdd(IngredientType item, int current)
	{
		for (var n = 0; n < Ingredients.Count; ++n)
		{
			if (item != Ingredients[n])
				continue;

			if (Counts[n] < current)
				return true;
		}

		return false;
	}
}