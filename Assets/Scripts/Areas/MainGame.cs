using System;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MarioObject
{
	private float _size;
	private float _height;

	protected override void Construct()
	{
		base.Construct();
	}

	protected override void Begin()
	{
		base.Begin();

		_size = Camera.main.orthographicSize;
		_height = Camera.main.transform.position.y;

		Camera.main.orthographicSize = 7.6f;
		Camera.main.transform.SetY(4.8f);

		Player.ShowCharacters(true);
	}

	public override void End()
	{
		base.End();
		Camera.main.orthographicSize = _size;
		Camera.main.transform.SetY(_height);

		Player.ShowCharacters(false);
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();
	}
}


