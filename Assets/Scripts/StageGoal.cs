﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A goal that the player must meet to reach next goal
/// </summary>
public class StageGoal : MonoBehaviour
{
	/// <summary>
	/// What is required for the goal to be completed
	/// </summary>
	public IngredientType[] Ingredients;

	/// <summary>
	/// Maximum game time to make all recipes
	/// </summary>
	public float Time;

	/// <summary>
	/// Name of the goal
	/// </summary>
	public string Name;

	public string MakeDescription()
	{
		return MakeDescription(this);
	}

	public static string MakeDescription(StageGoal goal)
	{
		return CreateText(MakeDict(goal));
	}

	private static string CreateText(Dictionary<IngredientType, int> dict)
	{
		var text = "Make ";
		var second = false;
		foreach (var kv in dict)
		{
			if (second)
				text += ", ";

			text += string.Format("{1} {0}s", kv.Key, kv.Value);
			second = true;
		}

		return text;
	}

	private static Dictionary<IngredientType, int> MakeDict(StageGoal goal)
	{
		var dict = new Dictionary<IngredientType, int>();
		foreach (var ing in goal.Ingredients)
		{
			if (!dict.ContainsKey(ing))
				dict.Add(ing, 0);

			dict[ing]++;
		}
		return dict;
	}
}