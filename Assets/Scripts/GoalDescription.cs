using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GoalDescription : MarioObject
{
	public Text Title;
	public Text Objective;
	public GameObject IngredientsPanel;

	public void ButtonPressed()
	{
		gameObject.SetActive(false);
	}

	public void ConstructPanel()
	{
		var goal = Player.CurrentGoal;
		Title.text = goal.Name;

		var dict = IngredientItem.CreateIngredientDict<int>();
		foreach (var i in goal.Ingredients)
			dict[i]++;

		var sb = new StringBuilder();
		sb.Append("Sell ");
		var second = false;
		foreach (var kv in dict)
		{
			if (kv.Value == 0)
				continue;

			if (second)
				sb.Append(", and ");

			second = true;

			sb.Append(string.Format("{0} {1}s", kv.Value, kv.Key));
		}

		Objective.text = sb.ToString();

		if (World == null)
			return;
		if (World.GoalPanel == null)
			return;

		World.GoalPanel.UpdateUi();
	}
}
