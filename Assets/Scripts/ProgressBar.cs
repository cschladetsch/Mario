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

	protected override void Begin()
	{
		base.Begin();

		_image = GetComponent<Image>();
	}

	protected override void Tick()
	{
		base.Tick();

		_time += RealDeltaTime;

		var delta = _time/TotalTime;

		_image.fillAmount = delta;
	}

	public void Reset()
	{
		Debug.Log("Bar.Reset");
		_time = 0;
		_image.fillAmount = 0;
		Paused = true;
	}
}
