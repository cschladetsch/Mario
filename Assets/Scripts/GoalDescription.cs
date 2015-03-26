using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the GoalDescription panel. Currently it's just text
/// TODO: Add icons for 2xA + 3xB = C
/// </summary>
public class GoalDescription : MarioObject
{
	public Text Title;
	public Text Objective;
	public GameObject IngredientsPanel;

	public void ButtonPressed()
	{
		World.CurrentArea.Paused = false;
		if (World.CurrentLevel)
			World.CurrentLevel.Pause(false);

		gameObject.SetActive(false);
	}

	public void ConstructPanel()
	{
		var goal = Player.CurrentGoal;
		Title.text = goal.Name;

		var dict = IngredientItem.CreateIngredientDict<int>();
		foreach (var i in goal.Ingredients)
			dict[i]++;

		Objective.text = BuildDescription(dict);

		World.Canvas.GoalPanel.UpdateUi();
	}

	private static String BuildDescription(Dictionary<IngredientType, int> dict)
	{
		var sb = new StringBuilder();
		sb.Append("Sell ");
		var second = false;
		foreach (var kv in dict.Where(kv => kv.Value != 0))
		{
			if (second)
				sb.Append(", and ");

			second = true;

			sb.Append(string.Format("{0} {1}s", kv.Value, kv.Key));
		}

		return sb.ToString();
	}
}