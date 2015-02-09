using UnityEngine;

/// <summary>
/// The overall controller for the game
/// </summary>
public class World : MonoBehaviour
{
	/// <summary>
	/// The current level
	/// </summary>
	public GameObject[] Levels;

	public Level Level;

	/// <summary>
	/// The single world instance
	/// </summary>
	public static World Instance;

	/// <summary>
	/// Current player
	/// </summary>
	public static Player Player;

	public static UiCanvas Canvas;

	/// <summary>
	/// The single truck
	/// </summary>
	public static Truck Truck;

	private bool _paused;

	private int _levelIndex;

	private bool _first = true;

	private int _beginLevel;

	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Can't have multiple Worlds");
			return;
		}

		Application.targetFrameRate = 60;

		Instance = this;

		Canvas = FindObjectOfType<UiCanvas>();

		_levelIndex = 0;
	}

	void Start()
	{
		//Startlevel();
		//Pause(true);
	}

	public void Reset()
	{
		if (Level == null)
			return;

		Truck.Reset();

		Level.Reset();

		Player.Reset();

		foreach (var c in FindObjectsOfType<Cake>())
			Destroy(c.gameObject);
	}

	public void Restart()
	{
		_levelIndex = 0;

		Reset();

		BeginLevel();
	}

	void Update()
	{
		// need to wait a few updates before beginning, because we can have nested SpawnGameObject components...
		if (_beginLevel > 0)
		{
			--_beginLevel;

			if (_beginLevel == 0)
			{
				BeginLevel();

				// if this is the first level, then we pause else we unpause
				Pause(_levelIndex == 0);
			}

			return;
		}

		if (_first)
		{
			_first = false;

			CreateLevel();

			Pause(true);
		}
	}

	public void Pause(bool pause)
	{
		if (pause == _paused)
			return;

		_paused = pause;

		foreach (var cake in FindObjectsOfType<Cake>())
			cake.Pause(pause);

		if (Level)
			Level.Pause(pause);
	}

	public void TogglePause()
	{
		Pause(!_paused);
	}

	public void CreateLevel()
	{
		Debug.Log("World.CreateLevel");

		if (Level)
			Destroy(Level.gameObject);

		var prefab = Levels[_levelIndex];
		Level = ((GameObject)Instantiate(prefab)).GetComponent<Level>();
		Level.transform.position = Vector3.zero;

		Level.Paused = true;

		//Level.gameObject.SetActive(false);

		// actually begin the level next update to allow nested spawners to complete
		_beginLevel = 5;
	}

	public void BeginLevel()
	{
		Player = FindObjectOfType<Player>();
		Truck = FindObjectOfType<Truck>();

		Reset();

		Pause(false);

		Level.BeginLevel();
		Level.Pause(false);
	}

	public void NextLevel()
	{
		//Debug.Log("Next Level");
		_levelIndex = (_levelIndex + 1)%Levels.Length;
		CreateLevel();
	}
}
