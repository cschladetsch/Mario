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

		var numCherries = ingredients[Ingredient.TypeEnum.Cherry];
		var numMuffins = ingredients[Ingredient.TypeEnum.Muffin];
		var num = Mathf.Min(numCherries, numMuffins);
		var numCupCakes = num*num;
		Debug.Log("Can make " + numCupCakes + " cupcakes");

		ingredients[Ingredient.TypeEnum.CupCake] += numCupCakes;

		Player.Gold += 5*numCupCakes;

		var rec = GameObject.Find("Recipies");
		var cc = rec.transform.FindChild("CupCake").GetComponent<Recipe>();

		Kernel.Factory.NewCoroutine(Cook, numCupCakes, cc);
	}

	private IEnumerator Cook(IGenerator self, int numCupCakes, Recipe recipe)
	{
		for (var n = 0; n < numCupCakes; ++n)
		{
			Debug.Log("Cooking");
			yield return self.ResumeAfter(TimeSpan.FromSeconds(recipe.CookingTime));
			Player.Ingredients[recipe.Result]++;
		}
	}

	protected override void Tick()
	{
		base.Tick();
	}
}


