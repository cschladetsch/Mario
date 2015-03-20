﻿using System.Collections.Generic;
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

		FindObjectOfType<IncomingPanel>().Reset();

		ToggleVisuals(true);

		Canvas.GoalPanel.gameObject.SetActive(false);
		Canvas.GoldPanel.gameObject.SetActive(false);
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