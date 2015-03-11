using UnityEngine;

/// <summary>
/// Common to all areas
/// </summary>
public class AreaBase : MarioObject
{
	/// <summary>
	/// The type of area this is
	/// </summary>
	public AreaType Type;

	/// <summary>
	/// The area-specific UI canvas associated with this area
	/// </summary>
	public GameObject UiCanvas;

	protected override void Begin()
	{
		base.Begin();
		if (UiCanvas)
			UiCanvas.SetActive(true);
	}

	public override void End()
	{
		base.Begin();
		if (UiCanvas)
			UiCanvas.SetActive(false);
	}

	public void Activate(bool activate)
	{
		//Debug.Log("Activate area " + name + ": " + activate);
		if (UiCanvas)
			UiCanvas.SetActive(activate);
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public virtual void EnterArea()
	{
		//Debug.Log("Area " + name + " entered");
	}

	public virtual void LeaveArea()
	{
		//Debug.Log("Area " + name + " left");
	}
}


