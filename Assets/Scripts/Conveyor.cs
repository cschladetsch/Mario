using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MarioObject
{
	/// <summary>
	/// Move speed of the conveyor
	/// </summary>
	public float Speed = 1;

	/// <summary>
	/// If false, we are moving from left to right
	/// </summary>
	public bool MoveRight;

	public float MinCakeSeparation = 1;

	/// <summary>
	/// The cakes currently on this conveyor
	/// </summary>
	public IList<Pickup> Contents
	{
		get { return _contents; }
	}

	/// <summary>
	/// The cakes on this conveyor
	/// </summary>
	public List<Pickup> _contents = new List<Pickup>();

	/// <summary>
	/// Cached collision box
	/// </summary>
	private BoxCollider2D _box;

	protected override void Begin()
	{
		base.Begin();
		_box = GetComponentInChildren<BoxCollider2D>();
	}

	/// <summary>
	/// Add a item to the conveyor
	/// </summary>
	/// <param name="item">the item to add</param>
	/// <param name="pos">where to add it, normalised to the length of the conveyor</param>
	public void AddItem(Pickup item, float pos)
	{
		//Debug.Log("AddItem " + item.name);
		item.Reset();
		item.Position = pos;
		item.Conveyor = this;

		_contents.Add(item);
	}

	public Vector3 GetStartLocation()
	{
		var loc = _box.bounds.min.x;
		if (!MoveRight)
			loc = _box.bounds.max.x;
		var pos = transform.position;
		return new Vector3(loc, pos.y, pos.z);
	}

	protected override void Tick()
	{
		//Debug.Log("Conveyor.Tick: " + UnityEngine.Time.frameCount);

		UpdateContents();

		MoveContents();
	}

	private void UpdateContents()
	{
		foreach (var item in _contents.ToList())
		{
			item.UpdateItem(MoveRight);

			if (item.Dropped)
				RemoveItem(item);
		}
	}

	private void MoveContents()
	{
		if (_contents.Count == 0)
			return;

		// sort by position in x
		_contents.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
		for (int n = 0; n < _contents.Count - 1; ++n)
		{
			var curr = _contents[n];
			var next = _contents[n + 1];

			if (Mathf.Abs(curr.transform.position.x - next.transform.position.x) < 0.01f)
			{
				curr.Position -= 0.05f;
			}
		}

		foreach (var item in _contents)
			MoveItem(item);
	}

	private bool MoveItem(Pickup item)
	{
		if (item.Hanging)
		{
			item.Moved = true;
			return false;
		}

		// ensure pickups do not stack on top of each other
		var move = EnsurePickupsDontStack(item);

		item.Moved = move;
		if (move)
			item.Position += Speed*GameDeltaTime;

		if (item.Position > 1)
		{
			item.StartHanging();
			return false;
		}

		var dist = item.Position*_box.bounds.size.x;
		var loc = _box.bounds.min.x + dist;
		if (!MoveRight)
			loc = _box.bounds.max.x - dist;

		item.gameObject.transform.position = new Vector3(loc, transform.position.y + 1, 0);

		return true;
	}

	private bool EnsurePickupsDontStack(Pickup item)
	{
		var mx = item.transform.position.x;
		var move = true;
		foreach (var c in _contents)
		{
			if (c == item)
				continue;

			var cx = c.transform.position.x;

			if (Mathf.Abs(cx - mx) < 0.01f)
				continue;

			if (MoveRight)
			{
				if (cx < mx)
					continue;

				if (cx - mx < MinCakeSeparation)
				{
					move = false;
					break;
				}
			}
			else
			{
				if (cx > mx)
					continue;

				if (mx - cx < MinCakeSeparation)
				{
					move = false;
					break;
				}
			}
		}

		return move;
	}

	public void RemoveItem(Pickup item)
	{
		_contents.Remove(item);
	}

	public void Pause(bool pause)
	{
		// Debug.Log("Conveyor "+name+" pause set to "+pause);
		Paused = pause;
	}

	public void Reset()
	{
		//Debug.Log("Level.Reset: Destroying all _contents");
		foreach (var c in _contents)
			Destroy(c.gameObject);

		_contents.Clear();
	}

	public void Clear()
	{
		_contents.Clear();
	}
}