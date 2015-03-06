using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Cache;
using Flow;
using UnityEngine;
using System.Collections;

/// <summary>
/// Something that makes a product given correct ingredients
/// </summary>
public class Cooker : MarioObject
{
	/// <summary>
	/// How to make the product that this cooker... cooks
	/// </summary>
	public Recipe Recipe;

	public Color DeselectedColor;

	public Color UnusableColor;

	/// <summary>
	/// True if this is currently selected cooker
	/// </summary>
	public bool Active;

	private UnityEngine.UI.Image _tint;

	private Dictionary<IngredientType, int> _ingredients;

	public GameObject ProgressBar;

	public void Select(bool select)
	{
		Debug.Log("Selected " + name + " " + select);
		_tint.color = DeselectedColor;
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		_ingredients = IngredientItem.CreateIngredientDict<int>();

		_tint = transform.FindChild("Tint").gameObject.GetComponent<UnityEngine.UI.Image>();

		Select(false);
	}

	/// <summary>
	/// Add an ingredient to the recipe
	/// </summary>
	/// <param name="type">the ingredient to add</param>
	/// <returns>true if was added</returns>
	public bool Add(IngredientType type)
	{
		for (var n = 0; n < Recipe.Ingredients.Count; ++n)
		{
			if (type != Recipe.Ingredients[n]) 
				continue;

			if (_ingredients[type] >= Recipe.Counts[n]) 
				continue;

			_ingredients[type]++;

			return true;
		}

		return false;
	}

	/// <summary>
	/// Remove an ingredient from the recipe
	/// </summary>
	/// <param name="type"></param>
	/// <returns>true if ingredient was removed</returns>
	public bool Remove(IngredientType type)
	{
		for (var n = 0; n < Recipe.Ingredients.Count; ++n)
		{
			if (type != Recipe.Ingredients[n])
				continue;

			if (_ingredients[type] == 0)
				continue;

			_ingredients[type]--;

			return true;
		}

		return false;	
	}

	public IFuture<bool> Cook()
	{
		if (!Recipe.Satisfied(_ingredients))
			return null;

		var future = Kernel.Factory.NewFuture<bool>();
		Kernel.Factory.NewCoroutine(Cook, future);
		return future;
	}

	public bool CanCook()
	{
		return Recipe.Satisfied(_ingredients);
	}

	IEnumerator Cook(IGenerator self, IFuture<bool> done)
	{
		var remaining = Recipe.CookingTime;
		while (remaining > 0)
		{
			remaining -= (float)Kernel.Time.Delta.TotalSeconds;
			UpdateProgressBar(remaining/Recipe.CookingTime);
			yield return 0;
		}

		done.Value = true;
	}

	private void UpdateProgressBar(float t)
	{
		Debug.Log("Cooking " + Recipe.Result + " in " + t);
	}

	public void RemoveIngredient(GameObject go)
	{
		var type = go.GetComponent<IngredientItem>().Type;
		_ingredients[type]--;
		Player.Ingredients[type]++;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		Debug.Log("Update Display");
	}

	public void IngredientButtonPressed(IngredientType item)
	{
		Debug.Log("Cooker added " + item);
		// TODO
		//if (Recipe.CanAdd(item))
		//	Recipe.Add(item);
	}

	public void CanUse(bool use)
	{
		Debug.Log("Can use cooker " + name + ": " + use);
	}

	public void StartBake()
	{
		Debug.Log("Start Bake");
	}
}
