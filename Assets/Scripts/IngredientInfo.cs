using UnityEngine;
using System.Collections;

/// <summary>
/// Information about an ingredient
/// </summary>
public class IngredientInfo : MonoBehaviour
{
	/// <summary>
	/// The type of ingredient
	/// </summary>
	public IngredientType Type;

	/// <summary>
	/// The visual image for it
	/// </summary>
	public Texture2D Image;

	/// <summary>
	/// CostText to buy. If zero, item cannot be bought
	/// </summary>
	public int Buy;

	/// <summary>
	/// Price to sell item at
	/// </summary>
	public int Sell;

	public int MinSpawnRate;

	public int MaxSpawnRate;

	///// <summary>
	///// How long to sell one of these items. NOT USED YET.
	///// </summary>
	//public float SellTimer = 3;
}
