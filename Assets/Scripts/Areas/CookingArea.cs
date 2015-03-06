using System;
using System.Collections;
using System.Collections.Generic;
using Flow;
using UnityEngine;

public class CookingArea : MarioObject
{
	public List<Recipe> Recipes = new List<Recipe>();

	public void IngredientButtonPressed(IngredientType type)
	{
		Debug.Log("Added a " + type);
		switch (type)
		{
			case IngredientType.Cherry:
				break;
		}
	}

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
	}

	public void IngredientButtonPressed(GameObject button)
	{
		//Debug.Log("Pressed " + button.name);
		//var item = button.GetComponent<IngredientItem>().Type;
	}

	protected override void Tick()
	{
		base.Tick();

	}
}
