using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MakeGlow : MarioObject
{
	public Color From = Color.yellow;
	public Color To = Color.white;
	public float Interval = 1;

	private float _timer;
	private bool _forward;
	private Image _image;
	private Button _button;

	protected override void Construct()
	{
		_button = GetComponent<Button>();
		if (_button == null)
			_image = GetComponent<Image>();

		if (!_button && !_image)
			Destroy(this);
	}

	protected override void Begin()
	{
		_forward = true;
		_timer = 0;
	}

	protected override void Tick()
	{
		_timer += GameDeltaTime;
		if (_timer > Interval)
		{
			_forward = !_forward;
			_timer = 0;
		}

		var color = _forward ? Color.Lerp(From, To, _timer) : Color.Lerp(To, From, _timer);
		if (_button)
		{
			var cb = _button.colors;
			cb.normalColor = color;
			_button.colors = cb;
			return;
		}

		_image.color = color;
	}
}
