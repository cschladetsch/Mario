using UnityEngine;

public class Parabola
{
	private float a, b, c;

	private float dx;

	public Vector3 FinalPos;

	public Parabola(Vector2 p1, Vector2 p2, Vector2 p3, float dx)
	{
		FinalPos = p3;

		this.dx = dx;

		var x1 = p1.x;
		var x2 = p2.x;
		var x3 = p3.x;

		var y1 = p1.y;
		var y2 = p2.y;
		var y3 = p3.y;

		var denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
		a = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
		b = (x3*x3 * (y1 - y2) + x2*x2 * (y3 - y1) + x1*x1 * (y2 - y3)) / denom;
		c = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;
	}

	public Vector2 Calc(float x)
	{
		x += dx*Time.deltaTime;
		return new Vector2(x, a*x*x + b*x + c);
	}
}
