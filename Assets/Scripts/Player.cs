using UnityEngine;

/// <summary>
/// The single player of the game, whom controls both Left and Right characters.
/// 
/// Keeps track of score and lives and pickups.
/// </summary>
public class Player : MarioObject
{
	/// <summary>
	/// How to make the left character
	/// </summary>
	public GameObject CharacterLeftPrefab;

	/// <summary>
	/// How to make the right character
	/// </summary>
	public GameObject CharacterRightPrefab;

	/// <summary>
	/// The control (input) system
	/// </summary>
	public Control Control;

	/// <summary>
	/// Where to store particle systems
	/// </summary>
	internal GameObject EmittingParticleSystems;

	/// <summary>
	/// The set of effects used by the player
	/// </summary>
	public GameObject Effects;

	/// <summary>
	/// If dead, the player cannot play anymore. Fact of life.
	/// </summary>
	public bool Dead { get { return Lives == 0; } }

	/// <summary>
	/// If true, the player doesn't lose a life if a cake is dropped or hits a bomb
	/// </summary>
	public bool GodMode = false;

	public delegate void CollisionHandler(Collision2D other);
	public delegate void TriggerHandler(Collider2D other);
	public delegate void PlayerEventHandler(Player player);

	public CollisionHandler OnCollision;
	public TriggerHandler OnTrigger;
	public PlayerEventHandler OnDied;
	public PlayerEventHandler OnCakeDropped;

	/// <summary>
	/// The left character
	/// </summary>
	public Character Left;

	/// <summary>
	/// The right character
	/// </summary>
	public Character Right;

	/// <summary>
	/// Cached access to the UI
	/// </summary>
	private UiCanvas _canvas;

	/// <summary>
	/// When a cake is dropped or you hit a bomb, you lose a life. 0 lives = game over
	/// </summary>
	public int Lives = 3;

	protected override void Construct()
	{
		Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();
	}

	protected override void Begin()
	{
		// TODO: attach to truck
		Left = transform.FindChild("Left").GetComponent<Character>();
		Right = transform.FindChild("Right").GetComponent<Character>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FindObjectOfType<World>().TogglePause();
		}
	}

	public void DroppedCake(Pickup pickup)
	{
		if (Dead || GodMode)
			return;

		if (pickup is Cake)
		{
			if (OnCakeDropped != null)
				OnCakeDropped(this);

			LoseLife();
		}
	}

	private void LoseLife()
	{
		if (--Lives == 0)
			Died();

		UpdateUi();
	}

	private void Died()
	{
		World.Pause(true);

		var score = int.Parse(_canvas.Score.text);

		if (SaveGame.UpdateHighScore(score))
			_canvas.ShowHighScore(score);
		else
			_canvas.ShowTapToStart();
	}

	private void UpdateUi()
	{
		_canvas.LivesRemaining.text = Lives.ToString();
	}

	public void Reset()
	{
		Lives = 3;

		UpdateUi();
	}

	public void BombHit(Bomb bomb)
	{
		LoseLife();
	}

	public void AddLife()
	{
		++Lives;

		UpdateUi();
	}
}
