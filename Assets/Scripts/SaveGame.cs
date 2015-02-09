using UnityEngine;

/// <summary>
/// Game state persistence
/// </summary>
public class SaveGame
{
	private const string HighScore = "HighScore";

	private const string HighLevel = "HighLevel";

	public static bool UpdateHighLevel(int score)
	{
		var curr = GetHighLevel();
		if (score < curr)
			return false;

		PlayerPrefs.SetInt(HighLevel, score);
		return true;
	}

	public static int GetHighLevel()
	{
		return PlayerPrefs.GetInt(HighLevel);
	}

	public static bool UpdateHighScore(int score)
	{
		var curr = GetHighScore();
		if (score < curr)
			return false;

		PlayerPrefs.SetInt(HighScore, score);
		return true;
	}

	public static int GetHighScore()
	{
		return PlayerPrefs.GetInt(HighScore);
	}
}
