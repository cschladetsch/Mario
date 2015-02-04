using UnityEngine;

public class HasWorld : MonoBehaviour
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

	/// <summary>
	/// The current level being played
	/// </summary>
	protected Level Level { get { return World.Level; } }

	public float Time;

	public float DeltaTime;

	void Awake()
	{
		Construct();
	}

	void Start()
	{
		World = World.Instance;

		Begin();
	}

	void Update()
	{
		if (Paused)
			return;

		var dt = UnityEngine.Time.deltaTime;
		DeltaTime = dt;
		Time += dt;

		Tick();
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
