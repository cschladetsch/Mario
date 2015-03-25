using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A progress bar that uses real time
/// </summary>
public class ProgressBar : MarioObject
{
	/// <summary>
	/// The total time for the bar
	/// </summary>
	public float TotalTime;

	/// <summary>
	/// If true, I have NFI
	/// </summary>
	public bool ShowTimer;

	/// <summary>
	/// Current real-time elapsed
	/// </summary>
	public float Elapsed
	{
		get { return _time; }
	}

	/// <summary>
	/// The image used to scale for progression
	/// </summary>
	private Image _image;

	/// <summary>
	/// Current real-time of the bar
	/// </summary>
	private float _time;

	/// <summary>
	/// Self-explanatory
	/// </summary>
	public float PercentFinished
	{
		get { return _time/TotalTime; }
	}

	public bool Completed
	{
		get { return _time >= TotalTime; }
	}

	public delegate void EndedHandler(ProgressBar pb);
	public event EndedHandler Ended;

	protected override void Begin()
	{
		base.Begin();

		_image = GetComponent<Image>();
		if (_image == null)
		{
			Debug.Log(transform.parent.name);
		}
	}

	public void SetPercent(float percent)
	{
		_image.fillAmount = percent;
	}

	protected override void Tick()
	{
		base.Tick();

		_time += RealDeltaTime;
		_image.fillAmount = Mathf.Min(1.0f, _time/TotalTime);

		UpdateEnded();
	}

	private void UpdateEnded()
	{
		if (_time < TotalTime)
			return;

		Paused = true;

		if (Ended != null)
			Ended(this);

		_time = 0;
	}

	public void Reset()
	{
		ResetRealTime();

		_time = 0;
		if (_image == null)
			_image = GetComponent<Image>();

		_image.fillAmount = 0;
		Paused = true;
	}

	public bool NextPeriod()
	{
		if (_time < TotalTime)
			return false;

		_time -= TotalTime;
		if (_time < 0)
			_time = 0;

		return true;
	}

	public void Reset(float elapsed)
	{
		Reset();
		_time = elapsed;
	}
}