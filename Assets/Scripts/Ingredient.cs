﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
	public enum TypeEnum
	{
		Cherry,
		Muffin,
		Chocolate,
		Raisen,
		Strawberry,
	}

	public TypeEnum Type;

	public float BaseCost;

	public string Name { get { return Type.ToString(); } }
}


