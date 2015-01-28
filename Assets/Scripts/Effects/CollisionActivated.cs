using UnityEngine;

public class CollisionActivated : Effect
{
	/// <summary>
	/// The layer on which 
	/// </summary>
	public int Layer;

	/// <summary>
	/// If true, we are triggered
	/// </summary>
	private bool _triggered;

	/// <summary>
	/// What caused the activation: either a collision or a collider trigger
	/// </summary>
	protected GameObject _reason;

	private Vector3 _where;

	void Start()
	{
		_player.OnCollision += CollisionEnter;
		_player.OnTrigger += TriggerEnter;
	}

	private void TriggerEnter(Collider2D other)
	{
		if (other.gameObject.layer == Layer)
		{
			//Debug.Log("Collsion " + name + " " + other.gameObject.name + " " + other.gameObject.layer + " " + Layer);
			_triggered = true;
			_reason = other.gameObject;
		}
	}

	private void CollisionEnter(Collision2D other)
	{
		if (other.gameObject.layer == Layer)
		{
			//Debug.Log("Collsion " + name + " " + other.gameObject.name + " " + other.gameObject.layer + " " + Layer);
			_triggered = true;
			_reason = other.gameObject;
		}
	}

	public override bool Triggered()
	{
		return _triggered;
	}

	void Update()
	{
		_triggered = false;
	}
}
