using UnityEngine;
using UnityEngine.UI;
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
	/// Prefab used to make a new image for this ingredient
	/// </summary>
	public GameObject ImagePrefab;

	/// <summary>
	/// How long it takes to cooks this item by default
	/// </summary>
	public float SellingTime = 50;

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
}