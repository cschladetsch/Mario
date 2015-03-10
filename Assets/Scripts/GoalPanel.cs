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

	private Sprite _tintSprite;

	protected override void BeforeFirstUpdate()
	{
		// retain so we can toggle it off and on
		_tintSprite = Tint.sprite;

		GatherPrefabsForIngredientDisplay();

		AddAllItems();

		SetGoal(Player.CurrentGoal);
	}

	private void GatherPrefabsForIngredientDisplay()
	{
		_prefabs[IngredientType.CupCake] = (GameObject) Resources.Load("Images/Cupcake");
		_prefabs[IngredientType.MintIceCream] = (GameObject) Resources.Load("Images/ChockMintIceCream");
	}

	public void SetGoal(StageGoal goal)
	{
		Debug.Log("Setting goal " + goal.Name);

		ButtonText.gameObject.SetActive(true);
		ButtonText.text = goal.MakeDescription();

		Tint.sprite = _tintSprite;
		ChangeOverlayColor(DimColor);

		// move to under tint so we get drawn brightly
		Tint.gameObject.SetActive(true);
		transform.parent = Tint.gameObject.transform;

		gameObject.SetActive(false);
	}

	public void GoalButtonPressed()
	{
		Debug.Log("Goal Button Pressed");
		ButtonText.transform.parent.gameObject.SetActive(false);

		Tint.sprite = null;
		ChangeOverlayColor(new Color(0,0,0,0));

		// from from under tint
		gameObject.SetActive(true);
		Tint.gameObject.SetActive(false);
		transform.parent = World.Canvas.transform;
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
		Debug.Log("Syncing goal panel");

		foreach (var kv in Player.Ingredients)
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
		Debug.Log(Player.CurrentGoal.Name);
		Debug.Log(Player.CurrentGoal.Ingredients);
		foreach (var kv in Player.CurrentGoal.Ingredients)
		{
			Debug.Log(kv);
		}

		ClearContents();

		var start = 180.0f;

		foreach (var ing in Player.CurrentGoal.Ingredients)
		{
			var type = ing;
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
			Destroy(c);
		_contents.Clear();
	}

	public void Refresh()
	{

		_contents.Clear();

		AddAllItems();
	}
}
