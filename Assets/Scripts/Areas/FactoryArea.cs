using UnityEngine;

/// <summary>
/// Controller for main game transitions
/// </summary>
public class FactoryArea : AreaBase
{
	/// <summary>
	/// size of camera ortho
	/// </summary>
	private float _size;

	/// <summary>
	/// height of camera
	/// </summary>
	private float _height;

	public override void EnterArea()
	{
		base.Begin();

		// used as a hack to adjust camera for main game area
		_size = Camera.main.orthographicSize;
		_height = Camera.main.transform.position.y;

		Camera.main.orthographicSize = 7.6f;
		Camera.main.transform.SetY(4.8f);

		Player.ShowCharacters(true);
	}

	public override void LeaveArea()
	{
		base.End();
		Camera.main.orthographicSize = _size;
		Camera.main.transform.SetY(_height);

		Player.ShowCharacters(false);
	}
}


