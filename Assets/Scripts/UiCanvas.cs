using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class UiCanvas : MarioObject
{
	public UnityEngine.UI.Text LivesRemaining;
	public UnityEngine.UI.Text Score;
	public UnityEngine.UI.Text CarTimer;
	public UnityEngine.UI.Text PlayerGoldText;

	public GameObject CarTimerObject;
	public GameObject TapToStart;
	public GameObject LevelCompleted;
	public GameObject HighScore;
	public GameObject PausedPanel;
	public GoalPanel GoalPanel;
	public GameObject TintPanel;

	protected override void Begin()
	{
		base.Begin();

		//TapToStart.SetActive(true);
		LevelCompleted = new GameObject("LevelCompleted");
		TapToStart = new GameObject("TapToStart");
		HighScore = new GameObject("HighScore");
		PausedPanel = new GameObject("PausedPanel");

		LevelCompleted.SetActive(false);
		HighScore.gameObject.SetActive(false);
		PausedPanel.gameObject.SetActive(false);

		TintPanel.SetActive(true);
	}

	public void Reset()
	{
		TapToStart.gameObject.SetActive(true);
	}

	public void Tapped()
	{
		TapToStart.gameObject.SetActive(false);
	}

	public void LevelCompletedTapped()
	{
		LevelCompleted.gameObject.SetActive(false);
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

	public void ShowTapToStart()
	{
		TapToStart.gameObject.SetActive(true);
	}

	public void PausedTapped()
	{
		PausedPanel.gameObject.SetActive(false);
		World.Pause(false);
	}

	public void UpdateGoldAmount()
	{
		if (Player == null)
		{
			//Debug.LogWarning("UiCanvas.UpdateGoldAmount: player is null");
			return;
		}

		PlayerGoldText.text = Player.Gold.ToString();
	}
}