using System;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MarioObject
{
	/// <summary>
	/// How long to wait before moving down a level
	/// </summary>
	public float MoveTime = 3;

	public int Height;

	public float SpasmLength = 1.0f;

	protected override void Tick()
	{
		base.Tick();

		//if (World.Player.Left.Height == Height)
		//{
		//	World.Player.Left.Spasm(SpasmLength);
		//}
	}
}