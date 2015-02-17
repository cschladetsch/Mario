﻿using UnityEngine;

public class Pickup : MonoBehaviour
{
	/// <summary>
	/// Parameterised position along conveyor: 0 is at start, 1 at end
	/// </summary>
	public float Position;

	/// <summary>
	/// How long to end at end of conveyor before falling
	/// </summary>
	public float HangTime = 4;

	public bool Moved;
	public Conveyor Conveyor;
	public float _hangTimer;
	public bool _dropped;

	/// <summary>
	/// Counts down after drop starts. Mostly for debugging
	/// </summary>
	public float _droppedTimer;

	/// <summary>
	/// True if currently hanging
	/// </summary>
	public bool Hanging { get { return _hanging; } }

	/// <summary>
	/// True if dropped after hanging.
	/// </summary>
	public bool Dropped { get { return _dropped; } }

	public float Width { get { return _box.bounds.extents.x*2; } }

	public float Height { get { return _box.bounds.extents.y*2; } }

	private BoxCollider2D _box;
	private bool _hanging;
	private bool _kine;
	private bool _firstDrop;


	void Awake()
	{
		_box = GetComponent<BoxCollider2D>();
		rigidbody2D.isKinematic = false;
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public void Create(Transform folder)
	{
	}

	public void StartHanging()
	{
		_hangTimer = HangTime;
		_hanging = true;
	}

	public void Reset()
	{
		if (Dropped)
			return;

		//_dropped = false;
		_hangTimer = 0;
		Position = 0;
		transform.localRotation = Quaternion.identity;
		_hanging = false;
	}

	public void Pause(bool pause)
	{
		if (pause)
		{
			_kine = rigidbody2D.isKinematic;
			rigidbody2D.isKinematic = true;
			return;
		}

		rigidbody2D.isKinematic = _kine;
	}

	internal void UpdateItem(bool moveRight)
	{
		if (UpdateDropped())
			return;

		UpdateHang(moveRight);
	}

	private bool UpdateDropped()
	{
		if (_droppedTimer > 0)
			_dropped = true;

		if (!Dropped)
			return false;

		_droppedTimer -= Time.time;
		if (_droppedTimer > 0)
			return false;

		Debug.LogWarning("Dropped cake not Destroyed!");
		FindObjectOfType<Player>().DroppedCake(this);
		if (Conveyor != null)
			Conveyor.RemoveItem(this);

		Destroy(gameObject);
		return true;
	}

	private void UpdateHang(bool moveRight)
	{
		if (!_hanging)
			return;

		transform.localRotation = Quaternion.Slerp(transform.localRotation,
			Quaternion.AngleAxis(moveRight ? -30 : 30, Vector3.forward), Time.deltaTime);

		_hangTimer -= Time.deltaTime;
		if (_hangTimer < 0)
			StartDropped(moveRight);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		var go = other.gameObject;
		if (Dropped && go.layer == 8)	// ground
		{
			HitGround();
			return;
		}

		if (!_firstDrop && !Dropped && go.layer == 9)	// conveyor
		{
			_firstDrop = true;
			//Debug.Log("Start: " + name);
			rigidbody2D.isKinematic = true;
			FindObjectOfType<Level>().GetConveyor(0).AddItem(this, 0.8f);
			return;
		}
	}

	protected virtual void StartDropped(bool moveRight)
	{
	}

	private void HitGround()
	{
		Destroy(gameObject);
	}

	public virtual void CharacterHit(Character character, Conveyor conv, Conveyor next)
	{
	}

	protected void Remove()
	{
		Conveyor.RemoveItem(this);
		Destroy(gameObject);
	}
}


