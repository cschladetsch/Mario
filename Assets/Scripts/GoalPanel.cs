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

	public StageGoal CurrentGoal;

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
	private readonly List<GoalngredientView> _contents = new List<GoalngredientView>();

	/// <summary>
	/// What to use to make the goal images. These should have GoalIngredientView components.
	/// They are stored in the Resources/Images folder
	/// </summary>
	private readonly Dictionary<IngredientType, GameObject> _prefabs = new Dictionary<IngredientType, GameObject>();

	protected override void BeforeFirstUpdate()
	{
		Ingredients = IngredientItem.CreateIngredientDict<int>();

		GatherPrefabsForIngredientDisplay();

		AddAllItems();

		//SetGoal(Player.CurrentGoal);
	}

	private void GatherPrefabsForIngredientDisplay()
	{
		_prefabs[IngredientType.CupCake] = (GameObject) Resources.Load("Images/CupcakeImage");
		_prefabs[IngredientType.MintIceCream] = (GameObject)Resources.Load("Images/MintIceCreamImage");

		Debug.Log("GoalPanel.GatherPrefabsForIngredientDisplay");
		foreach (var kv in _prefabs)
		{
			Debug.Log(String.Format("{0} {1}", kv.Key, kv.Value));
		}
	}

	public void SetGoal(StageGoal goal)
	{
		CurrentGoal = goal;
		Clear();
		AddAllItems();
	}

	public void GoalButtonPressed()
	{
		//World.GoalPanel.SetActive(false);
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
		Debug.Log("Goal.AddAllItems: " + Player.CurrentGoal.Name);
		//Debug.Log(Player.CurrentGoal.Ingredients);
		//foreach (var kv in Player.CurrentGoal.Ingredients)
		//	Debug.Log(kv);

		if (_prefabs.Count == 0)
			GatherPrefabsForIngredientDisplay();

		ClearContents();

		//var start = StartX;

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
			{
				Debug.LogWarning("No Prefab for " + type);
				continue;
			}

			var go = (GameObject) Instantiate(prefab);
			var view = go.GetComponent<GoalngredientView>();
			view.HasBeenReached(false);
			_contents.Add(view);

			go.transform.SetParent(transform, false);

			//Debug.Log("Added a " + type + " to the goal list");
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

	public void AddItem(IngredientType type)
	{
		Ingredients[type]++;
		UpdateUi();
	}

	public void Clear()
	{
		Ingredients = IngredientItem.CreateIngredientDict<int>();
		UpdateUi();
	}
}