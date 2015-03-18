using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the goal panel
/// </summary>
public class GoalPanel : MarioObject
{
	public Image Tint;

	public Dictionary<IngredientType, int> Ingredients;

	public Text ButtonText;

	/// <summary>
	/// Spacing between items
	/// </summary>
	public float Spacing = 42;

	/// <summary>
	/// Where to place first item
	/// </summary>
	public float StartX = 180.0f;

	/// <summary>
	/// What color to use as an overlay for everything else while goal accept button is being displayed
	/// </summary>
	public Color DimColor;

	/// <summary>
	/// What we have currently collected towards the goal
	/// </summary>
	readonly List<GoalngredientView> _contents = new List<GoalngredientView>();

	/// <summary>
	/// What to use to make the goal images. These should have GoalIngredientView components.
	/// They are stored in the Resources/Images folder
	/// </summary>
	readonly Dictionary<IngredientType, GameObject> _prefabs = new Dictionary<IngredientType, GameObject>();

	protected override void BeforeFirstUpdate()
	{
		Ingredients = IngredientItem.CreateIngredientDict<int>();

		GatherPrefabsForIngredientDisplay();

		AddAllItems();

		//SetGoal(Player.CurrentGoal);
	}

	public void Cooked(IngredientType item, int count)
	{
		Ingredients[item] += count;
		UpdateUi();
	}

	private void GatherPrefabsForIngredientDisplay()
	{
		_prefabs[IngredientType.CupCake] = (GameObject)Resources.Load("Images/Cupcake");
		_prefabs[IngredientType.MintIceCream] = (GameObject)Resources.Load("Images/ChockMintIceCream");
		//_prefabs[IngredientType.Chocolate] = (GameObject) Resources.Load("Images/ChockMintIceCream");
		//_prefabs[IngredientType.MintIceCream] = (GameObject) Resources.Load("Images/ChockMintIceCream");
	}

	public void SetGoal(StageGoal goal)
	{
		World.GoalPanel.SetActive(true);
	}

	public void GoalButtonPressed()
	{
		World.GoalPanel.SetActive(false);
	}

	private void ChangeOverlayColor(Color color)
	{
		transform.parent.GetComponent<Image>().color = color;
	}

	protected override void Begin()
	{
		base.Begin();
	}

	public void UpdateUi()
	{
		//Debug.Log("Syncing goal panel");

		if (Ingredients == null)
			Ingredients = IngredientItem.CreateIngredientDict<int>();

		foreach (var kv in Ingredients)
		{
			var type = kv.Key;
			var num = kv.Value;

			if (num == 0)
				continue;

			foreach (var c in _contents)
			{
				if (c.Type == type)
					c.HasBeenReached(true);

				if (--num == 0)
					break;
			}
		}
	}

	private void AddAllItems()
	{
		//Debug.Log(Player.CurrentGoal.Name);
		//Debug.Log(Player.CurrentGoal.Inventory);
		//foreach (var kv in Player.CurrentGoal.Inventory)
		//{
		//	Debug.Log(kv);
		//}
		if (_prefabs.Count == 0)
			GatherPrefabsForIngredientDisplay();

		ClearContents();

		//var tr = GetComponent<RectTransform>();

		var start = StartX;

		foreach (var ing in Player.CurrentGoal.Ingredients)
		{
			var type = ing;
			if (!_prefabs.ContainsKey(type))
			{
				Debug.LogWarning("GoalPanel.AddAllItems: Couldn't find prefab for " + type);
				continue;
			}

			var prefab = _prefabs[type];
			if (prefab == null)
				continue;

			//Debug.Log("Adding a " + type + " to the goal list");

			var go = (GameObject)Instantiate(prefab);
			var view = go.GetComponent<GoalngredientView>();
			view.HasBeenReached(false);
			_contents.Add(view);

			//go.transform.parent = Canvas.GoalPanel.transform;
			go.transform.SetParent(transform, false);
			go.transform.SetX(start);
			start += Spacing;

			//Debug.Log("Adding a " + view.Type + " to goals");
		}
	}

	private void ClearContents()
	{
		foreach (var c in _contents)
			Destroy(c.gameObject);

		_contents.Clear();
	}

	public void Refresh()
	{
		ClearContents();

		AddAllItems();
	}
}
