using System;
using System.Collections.Generic;
using Flow;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	private Dictionary<IngredientType, UnityEngine.UI.Text> _counts;

	public GameObject ProgressBar;

	protected override void Tick()
	{
		base.Tick();

		if (Generator == null && CanCook())
			Cook();
	}

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

		GatherIngredientButtons();
		//Select(false);
	}

	private void GatherIngredientButtons()
	{
		_counts = IngredientItem.CreateIngredientDict<UnityEngine.UI.Text>();
		foreach (Transform tr in transform)
		{
			var ing = tr.GetComponent<IngredientItem>();
			if (ing == null)
				return;

			var text = tr.FindChild("Count").GetComponent<UnityEngine.UI.Text>();
			_counts[ing.Type] = text;

			Debug.Log("Found Ingredient button for " + ing.Type + ": " + text.text);
		}
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

			_ingredients[type]++;
			_counts[type].text = _ingredients[type].ToString();

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
		if (_ingredients[type] == 0)
			return false;

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

	public IGenerator Generator;
	public IFuture<bool> Future;

	public delegate void CompletedHandler(IngredientType type);

	public event CompletedHandler Completed;

	public IFuture<bool> Cook()
	{
		if (!Recipe.Satisfied(_ingredients))
			return null;

		var future = Kernel.Factory.NewFuture<bool>();
		Generator = Kernel.Factory.NewCoroutine(Cook, future);
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

		for (var n = 0; n < Recipe.Ingredients.Count; ++n)
		{
			var type = Recipe.Ingredients[n];
			_ingredients[type] -= Recipe.Counts[n];
		}

		done.Value = true;

		UpdateDisplay();

		self.Complete();

		Generator = null;

		if (Completed != null)
			Completed(Recipe.Result);
	}

	private void UpdateProgressBar(float t)
	{
		Debug.Log("Cooking " + Recipe.Result + " in " + t);
	}

	public void RemoveIngredient(GameObject go)
	{
		var type = go.GetComponent<IngredientItem>().Type;
		if (!Remove(type))
			return;

		Player.Ingredients[type]++;
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		foreach (IngredientType type in Enum.GetValues(typeof (IngredientType)))
		{
			if (type == IngredientType.None)
				continue;

			_counts[type].text = _ingredients[type].ToString();
		}
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
