using System;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MarioObject
{
	/// <summary>
	/// How long to wait before moving down a level
	/// </summary>
	public float MoveTime = 3;

	public float Height;

	public float SpasmLength = 1.0f;

	protected override void Tick()
	{
		base.Tick();

		if (LeftCharacter.LevelHeight == Height)
		{
			LeftCharacter.Spasm(SpasmLength);
		}
	}
}
