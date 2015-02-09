﻿using UnityEngine;

public class UiCanvas : MarioObject
{
	public UnityEngine.UI.Text LivesRemaining;
	public UnityEngine.UI.Text Score;
	public GameObject TapToStart;
	public GameObject LevelCompleted;
	public GameObject HighScore;

	protected override void Begin()
	{
		base.Begin();
		TapToStart.SetActive(true);
		LevelCompleted.SetActive(false);
		HighScore.gameObject.SetActive(false);
	}

	public void Reset()
	{
		TapToStart.gameObject.SetActive(true);
	}

	public void Tapped()
	{
		TapToStart.gameObject.SetActive(false);
		World.BeginLevel();
	}

	public void LevelCompletedTapped()
	{
		LevelCompleted.gameObject.SetActive(false);
		World.NextLevel();
	}

	public void ShowHighScore(int score)
	{
		HighScore.gameObject.SetActive(true);
	}

	public void HighScoreTapped()
	{
		HighScore.gameObject.SetActive(false);
		World.Restart();
	}

	public void LevelEnded(Level level)
	{
		LevelCompleted.SetActive(true);
	}
}
