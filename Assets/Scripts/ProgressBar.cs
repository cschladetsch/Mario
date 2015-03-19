using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProgressBar : MarioObject
{
	public float TotalTime;

	public bool ShowTimer;

	private Image _image;

	private float _time;

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

		_time += GameDeltaTime;
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
	}

	public void Reset()
	{
		_time = 0;
		if (_image == null)
			_image = GetComponent<Image>();

		_image.fillAmount = 0;
		Paused = true;
	}
}