﻿using System.Collections.Generic;
using UnityEngine;

public class DeliveryTruck : MarioObject
{
	public bool Ready;

	private Transform _car;

	protected override void Construct()
	{
		base.Construct();
		_car = transform.FindChild("Visual").FindChild("DeliveryCar");
	}

	protected override void Begin()
	{
		base.Begin();
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

	}

	public Collider2D Collider;

	private bool _delivering;

	private float _endX, _speed;

	private Dictionary<IngredientType, int> _contents;

	public void Deliver(float startX, float endX, float time, float height, float depth, Dictionary<IngredientType, int> contents)
	{
		Canvas.CarTimerObject.gameObject.SetActive(true);
		_delivering = true;
		transform.position = new Vector3(startX, height, depth);
		_endX = endX;
		_speed = (_endX - startX)/time;
		_contents = contents;
		

		//Debug.Log("Delivering truck");
	}

	protected override void Tick()
	{
		base.Tick();

		MoveTimer();

		if (_delivering)
		{
			var delta = DeltaTime*_speed;
			transform.SetX(transform.position.x + delta);

			Ready = transform.position.x >= _endX;
			if (Ready)
			{
				_delivering = false;
				var text = Canvas.CarTimer;
				text.text = "Ready";
				text.color = Color.green;
			}
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

			Canvas.CarTimerObject.SetActive(false);
			Destroy(gameObject);

			World.BeginMainGame(_contents);
			//World.BeginArea(3);
		}
	}

	private void MoveTimer()
	{
		var point = Camera.main.WorldToScreenPoint(_car.position);
		var delta = _endX - transform.position.x;
		var time = delta/_speed;
		var text = string.Format("{0:0.0}s", time);

		Canvas.CarTimer.text = text;
		Canvas.CarTimer.color = Color.black;
		point.y += Collider.bounds.extents.y*15;
		Canvas.CarTimerObject.transform.position = point;
	}
}
