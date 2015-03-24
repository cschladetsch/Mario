using UnityEngine;


public class GoalngredientView : MarioObject
{
	public IngredientType Type;

	public void HasBeenReached(bool reached)
	{
		//Debug.Log("Reached a " + Type + ": " + reached);
		var c = reached ? 1 : 0.5f;
		var image = GetComponent<UnityEngine.UI.Image>();
		if (image == null)
		{
			Debug.LogWarning("GoalIngredientView for " + Type + " has no image component");
			return;
		}
		image.color = new Color(c, c, c);
	}
}