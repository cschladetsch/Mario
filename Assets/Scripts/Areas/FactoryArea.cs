using System.Runtime.InteropServices;
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
		Canvas.GoldPanel.gameObject.SetActive(false);

		//World.CurrentLevel.SpeedLevel = 0;

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
		Canvas.GoldPanel.gameObject.SetActive(true);
	}

	public void ToggleVisuals(bool on)
	{
		var level = World.CurrentLevel;
		if (!level)
			return;

		ToggleVisuals(level.gameObject, on);
	}
}