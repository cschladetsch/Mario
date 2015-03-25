using UnityEngine;

/// <summary>
/// Describes an arc which can be sampled at any given position
/// </summary>
public class ParabolaUI
{
	/// <summary>
	/// True if we're at the end-point given
	/// </summary>
	public bool Completed
	{
		get { return _alpha >= 1; }
	}

	/// <summary>
	/// Were we end up
	/// </summary>
	public Vector3 FinalPos;

	public Vector3 StartPos;

	public delegate void PositionChangedHandler(Vector2 pos);

	public delegate void EndedHandler(ParabolaUI para);

	public event PositionChangedHandler PositionChanged;
	public event EndedHandler Ended;

	// co-efficients
	private readonly float _a;
	private readonly float _b;
	private readonly float _c;

	// the delta-time to use
	private readonly float _startX;
	private readonly float _deltaX;

	// how long along the curve from 0..1
	internal float _alpha;

	public float _timeScale;

	public ParabolaUI(Vector2 p1, Vector2 p2, Vector2 p3, float timeScale)
	{
		StartPos = p1;
		FinalPos = p3;

		_deltaX = p3.x - p1.x;
		_startX = p1.x;
		_timeScale = timeScale;

		var x1 = p1.x;
		var x2 = p2.x;
		var x3 = p3.x;

		var y1 = p1.y;
		var y2 = p2.y;
		var y3 = p3.y;

		// see http://math.stackexchange.com/questions/405451/is-it-possible-to-find-out-x2-parabola-and-function-from-3-given-points
		_a = (y2*(x3 - x1) - y1*(x3 - x2) - y3*(x2 - x1))
			/(x1*x1*(x2 - x3) - x3*x3*(x2 - x1) - x2*x2*(x1 - x3));

		_b = (y2 - y1 + _a*(x1*x1 - x2*x2))
			/(x2 - x1);

		_c = -_a*x1*x1 - _b*x1 + y1;
	}

	/// <summary>
	/// Return a position based on game time delta
	/// </summary>
	/// <returns></returns>
	public Vector2 UpdatePos()
	{
		//_alpha += UnityEngine.Time.deltaTime*_timeScale;
		_alpha += Time.deltaTime*_timeScale;
		return CalcAt(_startX + _alpha*(FinalPos.x - StartPos.x));
	}

	public void Update()
	{
		if (_alpha > 1)
		{
			if (Ended != null)
				Ended(this);
			return;
		}

		var pos = UpdatePos();

		if (PositionChanged != null)
			PositionChanged(pos);
	}

	private Vector2 CalcAt(float x)
	{
		return new Vector2(x, _a*x*x + _b*x + _c);
	}
}