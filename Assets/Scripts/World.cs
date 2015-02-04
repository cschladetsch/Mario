using UnityEngine;

/// <summary>
/// The overall controller for the game
/// </summary>
public class World : MonoBehaviour
{
	/// <summary>
	/// The current level
	/// </summary>
	public Level Level;

	/// <summary>
	/// The single world instance
	/// </summary>
	public static World Instance;

	/// <summary>
	/// Current player
	/// </summary>
	public static Player Player;

	/// <summary>
	/// The single truck
	/// </summary>
	public static Truck Truck;

	private bool _paused;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Can't have multiple Worlds");
			return;
		}

		Application.targetFrameRate = 60;

		Instance = this;

		Level = FindObjectOfType<Level>();
		Player = FindObjectOfType<Player>();
		Truck = FindObjectOfType<Truck>();
	}

	void Start()
	{
		Pause(true);
	}

	public void Reset()
	{
		Truck.Reset();

		Level.Reset();

		Player.Reset();
	}

	void Update()
	{
	}

	public void Pause(bool pause)
	{
		if (pause == _paused)
			return;

		_paused = pause;

		foreach (var cake in FindObjectsOfType<Cake>())
			cake.Pause(pause);

		Level.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}

	public void StartGame()
	{
		Reset();

		Pause(false);

		Level.BeginLevel();
	}
}
