using UnityEngine;

/// <summary>
/// Game state persistence
/// </summary>
public class SaveGame
{
	/// <summary>
	/// Name of the key to use for highest score
	/// </summary>
	private const string HighScore = "HighScore";

	/// <summary>
	/// Name of the key to use for highest level
	/// </summary>
	private const string HighLevel = "HighLevel";

	/// <summary>
	/// 
	/// </summary>
	/// <param name="score"></param>
	/// <returns>the highest level reached</returns>
	public static bool UpdateHighLevel(int score)
	{
		var curr = GetHighLevel();
		if (score < curr)
			return false;

		PlayerPrefs.SetInt(HighLevel, score);
		return true;
	}

	/// <summary>
	/// The highest level reached
	/// </summary>
	/// <returns></returns>
	public static int GetHighLevel()
	{
		return PlayerPrefs.GetInt(HighLevel);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="score"></param>
	/// <returns>true if score was updated</returns>
	public static bool UpdateHighScore(int score)
	{
		var curr = GetHighScore();
		if (score < curr)
			return false;

		PlayerPrefs.SetInt(HighScore, score);
		return true;
	}

	/// <summary>
	/// Get latest highest score of local player
	/// </summary>
	/// <returns></returns>
	public static int GetHighScore()
	{
		return PlayerPrefs.GetInt(HighScore);
	}
}