using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A progress bar that uses real time
/// </summary>
public class ProgressBar : MarioObject
{
	public float TotalTime;

	public bool ShowTimer;

	public float Elapsed { get { return _time; } }

	private Image _image;

	private float _time;
	public float PercentFinished { get { return 1.0f - _time/TotalTime; } }

	public delegate void EndedHandler(ProgressBar pb);

	public event EndedHandler Ended;

	public bool Completed
	{
		get { return _time >= TotalTime; }
	}

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

		//Debug.Log("Bar Ended " + name);
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
