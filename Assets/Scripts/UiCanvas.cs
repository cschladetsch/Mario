using System;
using System.Collections.Generic;
using UnityEngine;

public class UiCanvas : HasWorld
{
	public UnityEngine.UI.Text LivesRemaining;
	public UnityEngine.UI.Text Score;
	public GameObject TapToStart;

	protected override void Begin()
	{
		base.Begin();
		TapToStart.SetActive(true);
	}

	public void Reset()
	{
		TapToStart.gameObject.SetActive(true);
	}

	public void Tapped()
	{
		TapToStart.gameObject.SetActive(false);
		World.StartGame();
	}
}
