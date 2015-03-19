using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for main game transitions
/// </summary>
public class FactoryArea : AreaBase
{
	/// <summary>
	/// size of camera ortho
	/// </summary>
	private float _size;

	/// <summary>
	/// height of camera
	/// </summary>
	private float _height;

	public override void EnterArea()
	{
		base.EnterArea();

		Player.ShowCharacters(true);

		World.CurrentLevel.ResetSpeed();

		ToggleVisuals(true);
	}

	public override void LeaveArea()
	{
		base.LeaveArea();
		//World.CurrentLevel.Truck.Paused = false;

		//Debug.LogWarning("Leaving Factory");
		//Camera.main.orthographicSize = _size;
		//Camera.main.transform.SetY(_height);

		Player.ShowCharacters(false);

		ToggleVisuals(false);
	}

	public void ToggleVisuals(bool on)
	{
		var level = World.CurrentLevel;
		if (!level)
			return;

		ToggleVisuals(level.gameObject, on);
	}
}