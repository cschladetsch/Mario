using Flow;
using UnityEngine;
using UnityEngine.UI;

public class IncomingPanel : MarioObject
{
	/// <summary>
	/// Scale of image, relative to screen size
	/// </summary>
	public float ImageScale = 0.12f;

	public void Populate()
	{
		Debug.Log("IncomingPanel.Popuplate: " + World.CurrentLevel.Inventory.Count);
	}

	public Vector3 DroppedCakePosition(IngredientType type)
	{
		Debug.Log("DroppedCakePosition");
		RemoveCake(type);
		return CurrentLevel.CakeSpawnPoint.transform.position;
	}

	public void Clear()
	{
		foreach (var go in gameObject.GetAllChildren())
			Destroy(go);
	}

	public Vector3 RemoveCake(IngredientType type)
	{
		//Debug.Log("IncomingPanel.RemoveCake: " + type);
		foreach (var cake in transform.GetComponentsInChildren<GoalngredientView>())
		{
			if (cake.Type != type)
				continue;

			var pos = cake.gameObject.GetRectTransform().position;

			Vector3 world;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetRectTransform(), pos, Camera.main, out world);

			Destroy(cake.gameObject);

			return world;
		}

		return Vector3.zero;
	}

	public void AddItems(IngredientType type, int count)
	{
		var prefab = World.GetInfo(type).ImagePrefab;
		for (var n = 0; n < count; ++n)
		{
			// make the image
			var go = (GameObject) Instantiate(prefab);
			go.transform.SetParent(transform);

			// why do I need to do this?
			go.transform.localScale = Vector3.one;
		}
	}

	public void Reset()
	{
		foreach (Transform e in transform)
		{
			Destroy(e.gameObject);
		}
	}
}