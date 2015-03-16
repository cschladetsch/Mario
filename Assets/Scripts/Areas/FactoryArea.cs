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

		//Debug.LogWarning("Entering Factory" + FrameCount);

		//// used as a hack to adjust camera for main game area
		//_size = Camera.main.orthographicSize;
		//_height = Camera.main.transform.position.y;

		//Camera.main.orthographicSize = 7.6f;
		//Camera.main.transform.SetY(4.8f);

		Player.ShowCharacters(true);

		//World.CurrentLevel.Truck.Paused = false;

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


