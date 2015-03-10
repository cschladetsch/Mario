using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GoalngredientView : MarioObject
{
	public IngredientType Type;

	protected override void Begin()
	{
		base.Begin();
	}

	public void HasBeenReached(bool reached)
	{
		//Debug.Log("Reached a " + Type + ": " + reached);
		var c = reached ? 1 : 0.5f;
		var image = GetComponent<UnityEngine.UI.Image>();
		if (image == null)
		{
			Debug.LogWarning("GoalIngredientView for " +  Type + " has no image component");
			return;
		}
		image.color = new Color(c, c, c);
	}
}
