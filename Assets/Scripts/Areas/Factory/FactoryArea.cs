using UnityEngine;

/// <summary>
/// Controller for main game transitions
/// </summary>
public class FactoryArea : AreaBase
{
	public override void EnterArea()
	{
		base.EnterArea();

		Player.ShowCharacters(true);

		World.CurrentLevel.ResetSpeed();

		FindObjectOfType<IncomingPanel>().Reset();

		ToggleVisuals(true);

		Canvas.GoalPanel.gameObject.SetActive(false);
		Canvas.PlayerGold.transform.parent.gameObject.SetActive(false);

		// dow we want to reset speed level when entering factory area?
		//World.CurrentLevel.SpeedLevel = 0;

		World.CurrentLevel.Paused = false;

		Truck.Reset();
	}

	protected override void Tick()
	{
		base.Tick();

#if DEBUG
		DeliverEverythingNow();
#endif
	}

	private void DeliverEverythingNow()
	{
		if (!Input.GetKeyDown(KeyCode.C))
			return;

		foreach (var c in FindObjectsOfType<Cake>())
			CurrentLevel.Truck.AddCake(c);

		foreach (var k in CurrentLevel.Conveyors)
			k.Clear();

		foreach (var kv in CurrentLevel.Inventory)
		{
			var type = kv.Key;
			var spawn = World.CurrentLevel.GetSpawner(type);
			if (spawn == null)
				continue;

			while (spawn.CanSpawn())
				CurrentLevel.Truck.AddCake(spawn.Spawn().GetComponent<Cake>());

			CurrentLevel.Truck.ForceDelivery();
		}

		CurrentLevel.Pause(true);
	}

	public override void LeaveArea()
	{
		base.LeaveArea();

		Player.ShowCharacters(false);

		ToggleVisuals(false);

		Canvas.GoalPanel.gameObject.SetActive(true);
		Canvas.PlayerGold.transform.parent.gameObject.SetActive(true);
	}

	public override void ItemCooked(IngredientType type)
	{
		base.ItemCooked(type);
		Debug.Log("Factory.ItemCooked: " + type);
	}

	public override void ItemSold(IngredientType type)
	{
		base.ItemSold(type);
		Debug.Log("Factory.ItemSold: " + type);
	}

	public void ToggleVisuals(bool on)
	{
		var level = World.CurrentLevel;
		if (!level)
			return;

		SetVisual(level.gameObject, on);
	}
}