using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Describes an arc which can be sampled at any given position
/// </summary>
public class Parabola
{
	/// <summary>
	/// Were we end up
	/// </summary>
	public Vector3 FinalPos;

	/// <summary>
	/// The start of the parabola
	/// </summary>
	public Vector3 StartPos;

	public float Interval;

	// co-efficients
	private readonly float _a;
	private readonly float _b;
	private readonly float _c;

	// the delta-time to use
	private float _time;

	public Parabola(Vector2 p1, Vector2 p2, Vector2 p3, float interval)
	{
		StartPos = p1;
		FinalPos = p3;

		Interval = interval;

		var x1 = p1.x;
		var x2 = p2.x;
		var x3 = p3.x;

		var y1 = p1.y;
		var y2 = p2.y;
		var y3 = p3.y;

		var denom = (x1 - x2)*(x1 - x3)*(x2 - x3);
		_a = (x3*(y2 - y1) + x2*(y1 - y3) + x1*(y3 - y2))/denom;
		_b = (x3*x3*(y1 - y2) + x2*x2*(y3 - y1) + x1*x1*(y2 - y3))/denom;
		_c = (x2*x3*(x2 - x3)*y1 + x3*x1*(x3 - x1)*y2 + x1*x2*(x1 - x2)*y3)/denom;
	}

	public Vector2 Calc()
	{
		//var delta = (FinalPos.x - StartPos.x);
		_time += Time.deltaTime;
		var x = _time*Interval;
		return Calc(x);
	}

	public Vector2 Calc(float x)
	{
		//x += _dx*Time.deltaTime;
		return new Vector2(x, _a*x*x + _b*x + _c);
	}

	public Vector2 CalcAtTime(float t)
	{
		var delta = FinalPos.x - StartPos.x;
		var x = delta*t;
		return Calc(StartPos.x + x);
	}
}