using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Make something "glow" by ping-pong interpolating between two colors.
/// 
/// First, it looks for a button and if it finds one, changes the "normal" color
/// of the button.
/// 
/// Next, it looks for an Image.
/// 
/// The colors change using a sinusoidal function to make it smooth.
/// </summary>
public class MakeGlow : MarioObject
{
	/// <summary>
	/// The first color
	/// </summary>
	public Color From = Color.yellow;

	/// <summary>
	/// The color to change to
	/// </summary>
	public Color To = Color.white;

	/// <summary>
	/// How long to change from one color to the other
	/// </summary>
	public float Interval = 1;

	/// <summary>
	/// How long in current transition
	/// </summary>
	private float _timer;

	/// <summary>
	/// If true, we're going From color To color
	/// </summary>
	private bool _forward;

	/// <summary>
	/// The image color we're changing
	/// </summary>
	private Image _image;

	/// <summary>
	/// The button color we're changing. If there is no Image or no Button found, the
	/// script self-destructs.
	/// </summary>
	private Button _button;

	protected override void Construct()
	{
		_button = GetComponent<Button>();
		if (_button == null)
		{
			_image = GetComponent<Image>();
			return;
		}

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

		var t = Mathf.Sin(_timer/Interval);

		var color = _forward ? Color.Lerp(From, To, t) : Color.Lerp(To, From, t);
		if (_button)
		{
			var cb = _button.colors;
			cb.normalColor = color;
			_button.colors = cb;
			return;
		}

		if (_image)
			_image.color = color;
	}
}