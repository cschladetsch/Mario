using System;
using Flow;
using UnityEngine;

/// <summary>
/// Base class for all objects in the game.
/// </summary>
public class MarioObject : MonoBehaviour
{
	/// <summary>
	/// If false, this game object will not be updated Tick() method
	/// </summary>
	public bool Paused;

	/// <summary>
	/// The single world object
	/// </summary>
	protected World World
	{
		get
		{
			if (_world != null)
				return _world;

			Awake();
			Start();

			return _world;
		}
	}

	private World _world;

	protected IKernel Kernel
	{
		get { return World.Kernel; }
	}

	public Player Player
	{
		get { return World.Player; }
	}

	public Truck Truck
	{
		get { return World.Truck; }
	}

	protected UiCanvas Canvas
	{
		get { return World.Canvas; }
	}

	protected internal Character LeftCharacter
	{
		get { return World.Player.Left; }
	}

	protected internal Character RightCharacter
	{
		get { return World.Player.Right; }
	}

	/// <summary>
	/// The current level being played
	/// </summary>
	protected Level CurrentLevel
	{
		get { return World.CurrentLevel; }
	}

	protected float GameTime;

	protected float GameDeltaTime;

	protected float RealTime;

	protected float RealDeltaTime;

	private bool _firstUpate = true;

	private void Awake()
	{
		_world = World.Instance;
		if (_world == null)
		{
			FindObjectOfType<World>().Awaken();
			_world = World.Instance;
			Construct();
			Start();
			return;
		}

		_world.Awaken();

		Construct();
	}

	private void Start()
	{
		//Debug.Log("Name=" + name + ", World=" + World);
		_lastTime = DateTime.Now;
		Begin();
	}

	public virtual void End()
	{
	}

	/// <summary>
	/// The last time a Tick() was executed
	/// </summary>
	private DateTime _lastTime;

	/// <summary>
	/// Only increases when Tick() is called, and object is not Paused
	/// </summary>
	protected int FrameCount;

	protected void ResetRealTime()
	{
		_lastTime = DateTime.Now;
		RealDeltaTime = 0;
		RealTime = 0;
	}

	private void Update()
	{
		var now = DateTime.Now;
		var delta = now - _lastTime;
		RealDeltaTime = (float) delta.TotalSeconds;
		_lastTime = now;
		RealTime += RealDeltaTime;

		if (Paused)
			return;

		FrameCount++;

		GameDeltaTime = Time.deltaTime;

		if (_firstUpate)
		{
			_firstUpate = false;

			BeforeFirstUpdate();

			//return;
		}

		Tick();

		GameTime += GameDeltaTime;
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