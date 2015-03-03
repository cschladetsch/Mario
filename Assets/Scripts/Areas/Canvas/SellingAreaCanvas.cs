using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class SellingAreaCanvas : MarioObject
{
	readonly Dictionary<Ingredient.TypeEnum, int> _contents = new Dictionary<Ingredient.TypeEnum, int>();

	/// <summary>
	/// Complete the order, remove the UI, start the truck
	/// </summary>
	public void FinishOrder()
	{
		Debug.Log("Finish Order");
		var area = World.CurrentArea as SellingArea;
		World.CurrentArea.UiCanvas.gameObject.SetActive(false);
		area.StartDeliveryTruck(_contents);
	}

	/// <summary>
	/// Order an in ingredient - or remove one. This is awkward because Unity 4.6 doesn't
	/// seem to allow for multiple arguments to OnCLick events any more
	/// </summary>
	/// <param name="button"></param>
	public void OrderIngredient(GameObject button)
	{
		var ing = button.transform.parent.GetComponent<Ingredient>().Type;
		var buttonName = button.GetComponent<UnityEngine.UI.Button>().name;
		var num = int.Parse(buttonName);
		Debug.Log(ing + " " +  num);

		if (!_contents.ContainsKey(ing))
			_contents.Add(ing, 0);

		var next = _contents[ing] + num;
		_contents[ing] = Mathf.Max(0, next);
	}
}


