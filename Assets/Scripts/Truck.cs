using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

public class Truck : MarioObject
{
	/// <summary>
	/// How fast to move when emptying
	/// </summary>
	public float MoveSpeed = 1;

	/// <summary>
	/// How far to move when emptying
	/// </summary>
	public float MoveDistance = 4;

	/// <summary>
	/// How long cakes take to fly into the truck
	/// </summary>
	public float FlightTime = 2;

	/// <summary>
	/// Number of columns for the stacked cakes
	/// </summary>
	public int NumColumns = 2;

	/// <summary>
	/// Number of rows for the stacked cakes
	/// </summary>
	public int NumRows = 3;

	public int NumCakes { get { return _cakes.Count + _pending.Count; } }

	public delegate void DeliveredHandler(Truck truck);

	public event DeliveredHandler DeliveryStarted;
	public event DeliveredHandler DeliveryCompleted;
	public int _numCakes;

	private Transform _pt;
	private readonly List<Cake> _cakes = new List<Cake>();
	private readonly List<Cake> _pending = new List<Cake>();
	private float _moveTime;
	private bool _movingLeft;
	private float _startPos;
	private bool _full;

	public bool Emptying;

	protected override void Construct()
	{
		base.Construct();
		_pt = transform.FindChild("FlythroughPoint");
	}

	protected override void Begin()
	{
		base.Begin();

		_startPos = transform.position.x;

		if (World.CurrentLevel == null)
			return;

		if (World.CurrentLevel.Inventory == null)
			return;

		_numCakes = World.CurrentLevel.Inventory.Sum(c => c.Value);
	}

	protected override void Tick()
	{
		base.Tick();

		if (!World.CurrentLevel)
			return;

		if (!World.CurrentLevel.Area)
			return;

		if (!World.CurrentLevel.Area.Visual)
			return;

		UpdateEmptying();

		if (Emptying)
			return;

		MoveCakes();

		UpdateDone();
	}

	private void UpdateEmptying()
	{
		if (!Emptying)
			return;

		var dt = GameDeltaTime;

		_moveTime -= dt;
		var p = transform.position;
		if (_movingLeft)
		{
			transform.position = new Vector3(p.x - MoveSpeed*dt, p.y, 0.0f);
			if (_moveTime <= 0)
			{
				Debug.Log("Truck done: " + World.CurrentLevel.NoMoreCakes);

				foreach (var kv in World.CurrentLevel.Inventory)
				{
					Debug.Log(string.Format("{0} {1}", kv.Key, kv.Value));
				}

				//if (World.CurrentLevel.NoMoreCakes)
				//{
				//	if (DeliveryCompleted != null)
				//		DeliveryCompleted(this);
				//	TransitionToBakery();
				//}
			}
			else
			{
				_movingLeft = false;
			}
			return;
		}

		// moving right
		transform.position = new Vector3(p.x + MoveSpeed*dt, p.y, 0.0f);
		if (_moveTime <= 0)
			EndEmptying();
	}

	//private void TransitionToBakery()
	//{
	//	EmptyCakes();

	//	// reset for the return trip
	//	_moveTime = MoveDistance/MoveSpeed;

	//	foreach (var c in _pending)
	//		Destroy(c.gameObject);

	//	foreach (var c in _cakes)
	//		Destroy(c.gameObject);

	//	_pending.Clear();
	//	_cakes.Clear();

	//	World.CurrentLevel.EndLevel();
	//	World.ChangeArea(AreaType.Bakery);
	//}

	private void EmptyCakes()
	{
		WriteScore();

		foreach (var cake in _cakes)
			Destroy(cake.gameObject);

		_cakes.Clear();
		_movingLeft = false;
	}

	private void WriteScore()
	{
		//var ui = FindObjectOfType<UiCanvas>();
		//var score = int.Parse(ui.Score.text);
		//score += _cakes.Count;
		//ui.Score.text = score.ToString();
	}

	private void EndEmptying()
	{
		if (!Emptying)
			return;

		transform.position = new Vector3(_startPos, transform.position.y, 0);

		Emptying = false;
		_full = false;

		foreach (var c in _pending.ToList())
		{
			AddCake(c);
			if (_full)
				break;

			_pending.Remove(c);
		}

		// if there was more than a truck-load pending, just delete the remainder.
		// this probably will never happen in a real game, but can happen in debugging modes
		foreach (var c in _pending)
		{
			Destroy(c.gameObject);
		}

		_pending.Clear();

		World.Pause(false);

		foreach (var c in _cakes)
			c.rigidbody2D.isKinematic = true;

		if (DeliveryCompleted != null)
			DeliveryCompleted(this);
	}

	private void UpdateDone()
	{
		var done = _numCakes == 0 && World.CurrentLevel.NoMoreCakes;
		if (!done)
			return;

		StartEmptying();
	}

	public void StartEmptying()
	{
		World.Pause(true);

		if (DeliveryStarted != null)
			DeliveryStarted(this);

		_moveTime = MoveDistance/MoveSpeed;
		_movingLeft = true;

		Emptying = true;
	}

	private void MoveCakes()
	{
		foreach (var cake in _cakes.ToList())
		{
			var para = cake.TruckParabola;
			if (para == null)	// already landed in truck
				continue;

			if (!cake)
			{
				Debug.LogWarning("Destroyed cake in truck move list");
				//Contents.Remove(cake);
				_cakes.Clear();
				return;
			}

			cake.transform.position = para.Calc(cake.transform.position.x);

			var arrived = cake.transform.position.x <= para.FinalPos.x;
			if (arrived)
			{
				CakeArrived(cake);
			}

			if (!arrived)
				continue;

			cake.transform.position = para.FinalPos;
			cake.TruckParabola = null;

			--_numCakes;
		}
	}

	private void CakeArrived(Cake cake)
	{
		AddToPlayerIngredients(cake);

		if (World.CurrentLevel.NoMoreCakes)
		{
			_movingLeft = true;
		}
	}

	private static void AddToPlayerIngredients(Cake cake)
	{
		World.Player.Inventory[cake.Type]++;
	}

	public void AddCake(Cake cake)
	{
		Debug.Log("Add cake");

		if (!cake)
		{
			Debug.LogError("Trying to add a deleted cake to Truck!");
			return;
		}

		cake.transform.parent = transform;
		cake.rigidbody2D.isKinematic = true;
		cake.transform.rotation = Quaternion.identity;

		if (_full)
		{
			Debug.Log("Full");
			_pending.Add(cake);
			return;
		}

		int row = _cakes.Count/NumColumns;
		int col = _cakes.Count%NumColumns;
		var width = cake.Width;
		var height = cake.Height;

		var finalPos = transform.position + new Vector3(-1, 1, 0);

		finalPos.x += col*width;
		finalPos.y += row*height;

		float delta = cake.transform.position.x - finalPos.x;
		float dx = -delta/FlightTime;
		cake.TruckParabola = new Parabola(cake.transform.position, _pt.position, finalPos, dx);

		cake.Position = 0;
		_cakes.Add(cake);
		//Debug.Log("Added cake type " + cake.Type + " called " + cake.name);
		if (_cakes.Count == NumColumns*NumRows)
		{
			_full = true;
		}
	}

	public void Reset()
	{
		foreach (var c in _pending)
			Destroy(c);

		foreach (var c in _cakes)
			Destroy(c);

		_pending.Clear();
		_cakes.Clear();
	}
}
