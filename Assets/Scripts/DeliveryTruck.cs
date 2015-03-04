using System.Collections.Generic;
using UnityEngine;

public class DeliveryTruck : MarioObject
{
	public bool Ready;

	//private Transform _car;

	protected override void Construct()
	{
		base.Construct();
		//_car = transform.FindChild("Visual").FindChild("DeliveryCar");
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	private bool _delivering;

	private float _endX, _speed;

	private Dictionary<Ingredient.TypeEnum, int> _contents;

	public void Deliver(float startX, float endX, float time, float height, float depth, Dictionary<Ingredient.TypeEnum, int> contents)
	{
		_delivering = true;
		transform.position = new Vector3(startX, height, depth);
		_endX = endX;
		_speed = (_endX - startX)/time;
		_contents = contents;

		//Debug.Log("Delivering truck");
	}

	/// <summary>
	/// Needless to say, this is just place-holder
	/// </summary>
	void OnGUI()
	{
		//var point = Camera.main.WorldToScreenPoint(_car.position);
		//var delta = _endX - transform.position.x;
		//var time = delta/_speed;
		//var remaining = string.Format("{0:0.0}s", time);
		//guiText.text = remaining;
	}

	protected override void Tick()
	{
		base.Tick();

		if (_delivering)
		{
			var delta = DeltaTime*_speed;
			transform.SetX(transform.position.x + delta);

			Ready = transform.position.x >= _endX;
			if (Ready)
				_delivering = false;
		}

		if (!Ready)
			return;

		if (!Input.GetMouseButtonDown(0)) 
			return;

		var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider != null)
		{
			//var other = hit.collider.gameObject;
			//var car = other.GetComponent<DeliveryTruck>();
			//if (!car)
			//	return;

			//Debug.Log("Hit Delivery Car");

			Destroy(gameObject);

			World.BeginMainGame(_contents);
		}
	}
}
