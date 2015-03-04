using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
	public enum TypeEnum
	{
		Cherry,
		Muffin,
		CupCake,

		Chocolate,
		Raisen,
		Strawberry,

		None,

	}

	public TypeEnum Type;

	public int BaseCost;

	public string Name { get { return Type.ToString(); } }

	public UnityEngine.UI.Text CostText;

	public void UpdateCostText()
	{
		CostText.text = BaseCost.ToString();
	}

}


