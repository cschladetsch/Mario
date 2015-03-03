using System;
using System.Collections.Generic;
using UnityEngine;

public class SellingAreaCanvas : MonoBehaviour
{
	public void FinishOrder()
	{
		Debug.Log("Finish Order");
		//World.NextArea();
	}

	public void OrderIngredient(GameObject button)
	{
		var ing = button.transform.parent.GetComponent<Ingredient>();
		var buttonName = button.GetComponent<UnityEngine.UI.Button>().name;
		Debug.Log(ing.Type + " " +  int.Parse(buttonName));
	}
}


