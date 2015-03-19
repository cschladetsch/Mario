using Flow;
using UnityEngine;
using UnityEngine.UI;
using wHiteRabbiT.Unity.Extensions;

public class IncomingPanel : MarioObject
{
	protected override void Construct()
	{
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
	}

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

	public Vector2 RemoveCake(IngredientType type)
	{
		//Debug.Log("IncomingPanel.RemoveCake: " + type);
		foreach (var cake in transform.GetComponentsInChildren<GoalngredientView>())
		{
			if (cake.Type != type)
				continue;

			var pos = cake.gameObject.GetRectTransform().position;

			Destroy(cake.gameObject);

			return pos;
		}

		return Vector2.zero;
	}

	public void AddItems(IngredientType type, int count)
	{
		var prefab = World.GetInfo(type).ImagePrefab;
		for (var n = 0; n < count; ++n)
		{
			var go = (GameObject) Instantiate(prefab);
			go.transform.SetParent(transform);
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