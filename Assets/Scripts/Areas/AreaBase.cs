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

	/// <summary>
	/// If true, the area may be running (Paused == false),
	/// but we want to turn off all visual elements
	/// </summary>
	public bool Visual;

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

	public virtual void SellItem(IngredientType type)
	{
	}

	protected override void Tick()
	{
		base.Tick();
	}

	public virtual void EnterArea()
	{
		//Debug.Log("Area " + name + " entered");
		UiCanvas.SetActive(true);
		Visual = true;
	}

	public virtual void LeaveArea()
	{
		//Debug.Log("Area " + name + " left");
		UiCanvas.SetActive(false);
		Visual = false;
	}

	public static void ToggleVisuals(GameObject root, bool on)
	{
		foreach (var go in root.GetAllChildren())
		{
			//Debug.Log("CHILD: " + go.name);

			var vis = go.name == "ArtContainer" || go.name == "Visual";
			if (!vis)
				continue;

			//Debug.Log("Showing " + go.transform.parent.name + ": " + on);

			go.gameObject.SetActive(on);
		}
	}

	/// <summary>
	/// Item has been sold by the player
	/// </summary>
	/// <param name="type"></param>
	protected void ItemSold(IngredientType type)
	{
		var info = World.GetInfo(type);
		Player.SoldItem(info);

		World.GoalPanel.AddItem(type);

		if (Player.GoalReached())
			World.NextGoal();
	}
}