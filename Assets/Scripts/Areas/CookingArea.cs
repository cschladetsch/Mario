using System;
using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

public class CookingArea : MarioObject
{
	public List<Recipe> Recipes = new List<Recipe>(); 

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		var ingredients = Player.Ingredients;
		foreach (var c in ingredients)
		{
			Debug.Log(" " + c.Key + ": " + c.Value);
		}

		var numCherries = ingredients[IngredientType.Cherry];
		var numMuffins = ingredients[IngredientType.Muffin];
		var num = Mathf.Min(numCherries, numMuffins);
		var numCupCakes = num*num;
		Debug.Log("Can make " + numCupCakes + " cupcakes");

		ingredients[IngredientType.CupCake] += numCupCakes;

		Player.Gold += 5*numCupCakes;

		var rec = GameObject.Find("Recipies");
		var cc = rec.transform.FindChild("CupCake").GetComponent<Recipe>();

		Kernel.Factory.NewCoroutine(Cook, numCupCakes, cc);
	}

	private IEnumerator Cook(IGenerator self, int numCupCakes, Recipe recipe)
	{

		Debug.Log("Before Cooking: " + Player.Ingredients[IngredientType.CupCake]);

		for (var n = 0; n < numCupCakes; ++n)
		{
			Debug.Log("Cooking " + recipe.Result);
			yield return self.ResumeAfter(TimeSpan.FromSeconds(recipe.CookingTime));

			for (var j = 0; j < recipe.Count1; ++j)
				Player.Ingredients[recipe.Item1]--;

			for (var j = 0; j < recipe.Count2; ++j)
				Player.Ingredients[recipe.Item2]--;

			for (var j = 0; j < recipe.Count3; ++j)
				Player.Ingredients[recipe.Item3]--;

			for (var j = 0; j < recipe.Count4; ++j)
				Player.Ingredients[recipe.Item4]--;

			Player.Ingredients[recipe.Result]++;
		}

		Debug.Log("Done Cooking: " + Player.Ingredients[IngredientType.CupCake]);

		World.BeginArea(0);
	}

	protected override void Tick()
	{
		base.Tick();
	}
}


