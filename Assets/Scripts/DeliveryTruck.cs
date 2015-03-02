using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCar : MarioObject
{
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

	protected override void Tick()
	{
		base.Tick();

		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (hit.collider != null)
		{
			Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
		}
	}
}


