using System;
using System.Collections.Generic;
using UnityEngine;

public class UiCanvas : MarioObject
{
	public UnityEngine.UI.Text LivesRemaining;
	public UnityEngine.UI.Text Score;
	public GameObject TapToStart;
	public GameObject LevelCompleted;

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

	public void LevelEnded(Level level)
	{
	}
}
