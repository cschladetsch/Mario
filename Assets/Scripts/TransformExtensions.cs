using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	public static void SetX(this Transform tr, float x)
	{
		var p = tr.position;
		tr.position = new Vector3(x, p.y, p.z);
	}

	public static void SetY(this Transform tr, float y)
	{
		var p = tr.position;
		tr.position = new Vector3(p.x, y, p.z);
	}

	public static void SetZ(this Transform tr, float z)
	{
		var p = tr.position;
		tr.position = new Vector3(p.x, p.y, z);
	}
}


