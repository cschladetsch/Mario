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

	//private UnityEngine.UI.Image _tint;

	private Dictionary<IngredientType, int> _ingredients;
	private Dictionary<IngredientType, UnityEngine.UI.Text> _counts;

	public UnityEngine.UI.Text TimerText;

	//public GameObject ProgressBar;

	protected override void Tick()
	{
		base.Tick();

		if (!_cooking && CanCook())
			Cook();
	}

	public void Select(bool select)
	{
		Debug.Log("Selected " + name + " " + select);
		//_tint.color = DeselectedColor;
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		_ingredients = IngredientItem.CreateIngredientDict<int>();

		//_tint = transform.FindChild("Tint").gameObject.GetComponent<UnityEngine.UI.Image>();

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

			//Debug.Log("Found Ingredient button for " + ing.Type + ": " + text.text);
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
		//if (_ingredients[type] == 0)
		//	return false;
		//_ingredients[type]--;
		return true;
	}

	public IGenerator Generator;
	public IFuture<bool> Future;

	public delegate void CompletedHandler(IngredientType type);

	public event CompletedHandler Completed;

	public IFuture<bool> Cook()
	{
		if (_cooking)
			return null;

		if (!Recipe.Satisfied(_ingredients))
			return null;

		var future = Kernel.Factory.NewFuture<bool>();
		Generator = Kernel.Factory.NewCoroutine(Cook, future);
		return future;
	}

	public bool CanCook()
	{
		return Recipe.Satisfied(_ingredients) && !_cooking;
	}

	private bool _cooking;

	IEnumerator Cook(IGenerator self, IFuture<bool> done)
	{
		if (_cooking)
		{
			self.Complete();
			yield break;
		}

		Debug.Log("Cooking a " + Recipe.Result + " " + UnityEngine.Time.frameCount);

		_cooking = true;
		//ProgressBar.transform.localScale = new Vector3(0,1,1);

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
			Debug.Log(string.Format("Removing {0} {1} from {2}", Recipe.Counts[n], type, _ingredients[type]));
			_ingredients[type] -= Recipe.Counts[n];

			// HACK FRI
			if (_ingredients[type] < 0)
				_ingredients[type] = 0;
		}

		done.Value = true;

		self.Complete();

		Generator = null;

		if (Completed != null)
			Completed(Recipe.Result);

		Debug.Log("Cooked a " + Recipe.Result);

		Player.Ingredients[Recipe.Result]++;

		UpdateDisplay();

		_cooking = false;
	}

	private void UpdateProgressBar(float t)
	{
		TimerText.text = string.Format("{0:0.0}", t);
		//// 1.25

		//var y = ProgressBar.transform.position.y;
		//var len = t*1.25f;

		//ProgressBar.transform.localScale = new Vector3(len, 1, 1);
		//ProgressBar.transform.SetX(len/2.0f);
		////Debug.Log("Cooking " + Recipe.Result + " in " + t);
	}

	public void RemoveIngredient(GameObject go)
	{
		var type = go.GetComponent<IngredientItem>().Type;
		Debug.Log("Remove " + type);
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

			if (_counts[type] == null)
				continue;

			_counts[type].text = _ingredients[type].ToString();
		}

		var ui = transform.parent.GetComponent<CookingAreaUI>();
		ui.InventoryPanel.UpdateDisplay(Player.Ingredients, false);
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

	public bool Cooking()
	{
		return _cooking;
	}
}
