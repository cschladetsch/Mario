using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the goal panel
/// </summary>
public class GoalPanel : MarioObject
{
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
	readonly List<GoalngredientView> _requirements = new List<GoalngredientView>();

	/// <summary>
	/// What to use to make the goal images. These should have GoalIngredientView components.
	/// They are stored in the Resources/Images folder
	/// </summary>
	readonly Dictionary<IngredientType, GameObject> _prefabs = new Dictionary<IngredientType, GameObject>();

	protected override void BeforeFirstUpdate()
	{
		GatherPrefabsForIngredientDisplay();

		AddAllItems();
	}

	public void Cooked(IngredientType item, int count)
	{
		UpdateUi();
	}

	private void GatherPrefabsForIngredientDisplay()
	{
		_prefabs[IngredientType.CupCake] = (GameObject)Resources.Load("Images/Cupcake");
		_prefabs[IngredientType.MintIceCream] = (GameObject)Resources.Load("Images/ChockMintIceCream");
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

		if (World.Player == null || World.Player.SoldItems == null)
			return;

		foreach (var kv in World.Player.SoldItems)
		{
			var type = kv.Key;
			var num = kv.Value;

			if (num == 0)
				continue;

			foreach (var c in _requirements)
			{
				if (c.Type == type)
					c.HasBeenReached(true);

				if (--num == 0)
					break;
			}
		}
	}

	/// <summary>
	/// Add required items to goal panel
	/// </summary>
	private void AddAllItems()
	{
		if (_prefabs.Count == 0)
			GatherPrefabsForIngredientDisplay();

		ClearContents();

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
			_requirements.Add(view);

			//go.transform.parent = Canvas.GoalPanel.transform;
			go.transform.SetParent(transform, false);
			go.transform.SetX(start);
			start += Spacing;

			//Debug.Log("Adding a " + view.Type + " to goals");
		}

		UpdateUi();
	}

	private void ClearContents()
	{
		foreach (var c in _requirements)
			Destroy(c.gameObject);

		_requirements.Clear();

		UpdateUi();
	}

	public void Refresh()
	{
		ClearContents();

		AddAllItems();

		UpdateUi();
	}
}
