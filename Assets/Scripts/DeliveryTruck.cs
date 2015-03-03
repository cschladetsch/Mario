using UnityEngine;

public class DeliveryTruck : MarioObject
{
	public bool Ready;

	protected override void Construct()
	{
		base.Construct();
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

	public void Deliver(float startX, float endX, float time, float height, float depth)
	{
		_delivering = true;
		transform.position = new Vector3(startX, height, depth);
		_endX = endX;
		_speed = (_endX - startX)/time;

		Debug.Log("Delivering truck");
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
			Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
			Destroy(gameObject);

			World.BeginArea(2);
		}
	}
}
