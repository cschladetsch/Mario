using UnityEngine;

/// <summary>
/// Base class for all objects in the game.
/// </summary>
public class MarioObject : MonoBehaviour
{
	/// <summary>
	/// If false, this game object will not be updated via Update() method
	/// </summary>
	public bool Paused;

	/// <summary>
	/// The single world object
	/// </summary>
	protected World World;

	protected Player Player { get { return World.Player; } }

	protected Truck Truck { get { return World.Truck; } }

	protected UiCanvas Canvas { get { return World.Canvas; } }

	protected internal Character LeftCharacter { get { return World.Player.Left; } }

	protected internal Character RightCharacter { get { return World.Player.Right; } }

	/// <summary>
	/// The current level being played
	/// </summary>
	protected Level Level { get { return World.Level; } }

	protected float Time;

	protected float DeltaTime;

	private bool _firstUpate = true;

	void Awake()
	{
		Construct();
	}

	void Start()
	{
		World = World.Instance;
		//Debug.Log("Name=" + name + ", World=" + World);

		Begin();
	}

	void Update()
	{
		//// WHAT THE FUCK: why is Update() called twice on MarioObjects?
		//var frameCount = UnityEngine.Time.frameCount;
		//Debug.Log("MarioObject.Update: " + name + ", " + frameCount + ", " + _updateFrame + ", " + gameObject.GetInstanceID());
		//if (frameCount == _updateFrame && _updateFrame > 0)
		//	return;

		if (Paused)
			return;

		var dt = UnityEngine.Time.deltaTime;
		DeltaTime = dt;

		if (_firstUpate)
		{
			_firstUpate = false;

			BeforeFirstUpdate();

			return;
		}

		Tick();

		Time += dt;
	}

	protected virtual void BeforeFirstUpdate()
	{
	}

	protected virtual void Tick()
	{
	}

	protected virtual void Construct()
	{
	}

	protected virtual void Begin()
	{
	}
}
