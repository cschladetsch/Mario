using System.Collections.Generic;
using System.Linq;
using Flow;
using UnityEngine;

#pragma warning disable 414

/// <summary>
/// The behavior of the truck in the conveyor mini-game
/// </summary>
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
	public float FlightTime = 0.5f;

	/// <summary>
	/// Number of columns for the stacked cakes
	/// </summary>
	public int NumColumns = 2;

	/// <summary>
	/// Number of rows for the stacked cakes
	/// </summary>
	public int NumRows = 3;

	/// <summary>
	/// If true, Truck is currently delivering items to Player
	/// </summary>
	public bool Emptying;

	/// <summary>
	/// The number of cakes in the truck, or moving to the truck
	/// </summary>
	public int NumCakes
	{
		get { return _cakes.Count; }
	}

	public delegate void DeliveredHandler(Truck truck);

	public event DeliveredHandler DeliveryStarted;
	public event DeliveredHandler DeliveryCompleted;

	private Transform _flyThroughPoint;
	private readonly List<Cake> _cakes = new List<Cake>();
	private float _moveTime;
	private bool _movingLeft;
	private float _startPos;

	protected override void Construct()
	{
		base.Construct();

		_flyThroughPoint = transform.FindChild("FlythroughPoint");
	}

	protected override void Begin()
	{
		base.Begin();

		Emptying = false;
		_startPos = transform.position.x;
	}

	protected override void Tick()
	{
		base.Tick();

		if (!World.CurrentLevel || !World.Areas[AreaType.Factory].Visual)
			return;

		UpdateCheckFull();

		UpdateEmptying();

		if (!Emptying)
			MoveCakes();
	}

	private void UpdateCheckFull()
	{
		if (Emptying)
			return;

		if (World.CurrentLevel.NothingToSpawn)
		{
			if (World.CurrentLevel.Conveyors.Any(c => c.NumCakes > 0))
			{
				return;
			}
			//foreach (var c in World.CurrentLevel.Conveyors)
			//{
			//	if (c.NumCakes > 0)
			//	{
			//		Debug.Log(c.name + " has " + c.NumCakes);
			//		foreach (var k in c.Contents)
			//		{
			//			Debug.Log(k.Type + " is cake: " + Cake.Is(k.Type));
			//		}
			//		return;
			//	}
			//}
		}

		if (_cakes.Count > 0)
		{
			if (World.CurrentLevel.NoMoreCakes && _cakes.All(c => c.Delivered))
				StartEmptying();
		}
	}

	/// <summary>
	/// Move the truck left, deliver all items to player's inventory, clear contents, then
	/// move back to the start position
	/// </summary>
	private void UpdateEmptying()
	{
		if (!Emptying)
			return;

		//Debug.Log("UpdateEmptying: " + " left=" + _movingLeft + " moveTime=" + _moveTime + " , dt=" + GameDeltaTime);
		var dt = GameDeltaTime;

		_moveTime -= dt;
		var p = transform.position;

		if (_movingLeft)
		{
			MoveLeft(p, dt);
			return;
		}

		MoveRight(p, dt);
	}

	private void MoveRight(Vector3 p, float dt)
	{
		transform.position = new Vector3(p.x + MoveSpeed*dt, p.y, 0.0f);
		if (_moveTime <= 0)
			EndEmptying();
	}

	private void MoveLeft(Vector3 p, float dt)
	{
		transform.position = new Vector3(p.x - MoveSpeed*dt, p.y, 0.0f);
		if (_moveTime > 0)
			return;

		CompleteDeliveryImmediately();
	}

	private void CompleteDeliveryImmediately()
	{
		CompleteDelivery();
	}

	private void CompleteDelivery()
	{
		if (DeliveryCompleted != null)
			DeliveryCompleted(this);

		//Debug.Log("Delivery completed: " + _cakes.Count);
		EndEmptying();
	}

	private void StartMovingRight()
	{
		_moveTime = CalcMoveTime();
		_movingLeft = false;
	}


	/// <summary>
	/// Truck has moved to the left, delivered all contents to player,
	/// and has returned to start pos, empty
	/// </summary>
	private void EndEmptying()
	{
		if (!Emptying)
			return;

		//Debug.Log("Ending emptying");

		transform.position = new Vector3(_startPos, transform.position.y, 0);

		Emptying = false;

		World.Pause(false);

		World.ChangeArea(AreaType.Bakery);

		//Debug.Log("Truck.EndEmptying: " + _cakes.Count);
		World.BakeryArea.TakeDelivery(_cakes).Completed += f =>
		{
			//Debug.Log("TakeDelivery Completed");

			foreach (var c in _cakes)
				Destroy(c.gameObject);
			_cakes.Clear();

			Truck.Reset();
			World.BakeryArea.DeliveryTruck.Reset();
			World.Pause(false);
		};
	}

	public void StartEmptying()
	{
		if (Emptying)
			return;

		//Debug.Log("Truck.StartEmptying");
		World.Pause(true);

		if (DeliveryStarted != null)
			DeliveryStarted(this);

		_moveTime = CalcMoveTime();
		_movingLeft = true;

		Emptying = true;
	}

	private float CalcMoveTime()
	{
		return MoveDistance/MoveSpeed;
	}

	private void MoveCakes()
	{
		foreach (var cake in _cakes.ToList().Where(cake => !cake.Delivered))
		{
			if (!cake)
			{
				Debug.LogWarning("Destroyed cake in truck move list");
				_cakes.Clear();
				return;
			}

			var para = cake.TruckParabola;
			cake.transform.position = para.UpdatePos();

			if (para.Completed)
			{
				cake.transform.position = para.FinalPos;
				cake.TruckParabola = null;
				cake.Delivered = true;
			}
		}
	}

	//private void CakeArrived(Cake cake)
	//{
	//	if (World.CurrentLevel.NoMoreCakes)
	//	{
	//		_movingLeft = true;
	//	}
	//}

	/// <summary>
	/// Add a cake to the truck. Each new cake starts off moving to the truck,
	/// only when it arrives is it considered 'on the truck'.
	/// </summary>
	/// <param name="cake">the cake to add to the truck</param>
	public void AddCake(Cake cake)
	{
		//Debug.Log("Add cake");

		if (Emptying)
		{
			cake.Drop();
			return;
		}

		if (!cake)
		{
			Debug.LogError("Trying to add a deleted cake to Truck!");
			return;
		}

		cake.transform.parent = transform;
		cake.rigidbody2D.isKinematic = true;
		cake.transform.rotation = Quaternion.identity;

		int row = _cakes.Count/NumColumns;
		int col = _cakes.Count%NumColumns;
		var width = cake.Width;
		var height = cake.Height;

		var finalPos = transform.position + new Vector3(-1, 1, 0);

		finalPos.x += col*width;
		finalPos.y += row*height;

		Debug.DrawLine(cake.transform.position, _flyThroughPoint.position, Color.green, 2);
		Debug.DrawLine(_flyThroughPoint.position, finalPos, Color.red, 2);
		cake.TruckParabola = new ParabolaUI(cake.transform.position, _flyThroughPoint.position, finalPos, FlightTime);

		cake.Position = 0;
		_cakes.Add(cake);
	}

	public void Reset()
	{
		//Debug.Log("Truck.Reset");
		foreach (var c in _cakes)
			Destroy(c.gameObject);

		_cakes.Clear();
	}

	/// <summary>
	/// Deliver whatever is in the truck, as long as there
	/// are no cakes in transition to truck
	/// </summary>
	public void Deliver()
	{
		if (_cakes.Count > 0 && _cakes.All(c => c.Delivered))
			StartEmptying();
	}

	public void ForceDelivery()
	{
		foreach (var c in _cakes)
			c.Delivered = true;

		Emptying = false;
		StartEmptying();
	}
}