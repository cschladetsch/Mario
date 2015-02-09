using UnityEngine;

public class Player : MarioObject
{
	public GameObject CharacterLeftPrefab;

	public GameObject CharacterRightPrefab;

	public Control Control;

	internal GameObject LeftCharacter, RightCharacter;

	internal GameObject EmittingParticleSystems;

	public GameObject Effects;

	public bool Dead { get { return Lives == 0; } }

	public bool GodMode = false;

	public delegate void CollisionHandler(Collision2D other);
	public delegate void TriggerHandler(Collider2D other);
	public delegate void PlayerEventHandler(Player player);

	public CollisionHandler OnCollision;
	public TriggerHandler OnTrigger;
	public PlayerEventHandler OnDied;
	public PlayerEventHandler OnCakeDropped;

	private UiCanvas _canvas;

	public int Lives = 3;

	protected override void Begin()
	{
		// TODO: attach to truck
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FindObjectOfType<World>().TogglePause();
		}
	}

	protected override void Construct()
	{
		Control = GetComponent<Control>();
		_canvas = FindObjectOfType<UiCanvas>();
	}

	public void DroppedCake()
	{
		if (!Dead && OnCakeDropped != null)
			OnCakeDropped(this);

		if (Dead || GodMode)
			return;

		if (--Lives == 0)
			Died();

		UpdateUi();
	}

	private void Died()
	{
		World.Pause(true);
		_canvas.TapToStart.SetActive(true);
		//SaveGame.UpdateHighScore(_score);
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
}


