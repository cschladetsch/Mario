/// <summary>
/// Controller for main game transitions
/// </summary>
public class FactoryArea : AreaBase
{
	public override void EnterArea()
	{
		base.EnterArea();

		Player.ShowCharacters(true);

		World.CurrentLevel.ResetSpeed();

		FindObjectOfType<IncomingPanel>().Reset();

		ToggleVisuals(true);

		Canvas.GoalPanel.gameObject.SetActive(false);
		Canvas.GoldPanel.gameObject.SetActive(false);

		//World.CurrentLevel.SpeedLevel = 0;
	}

	public override void LeaveArea()
	{
		base.LeaveArea();

		Player.ShowCharacters(false);

		ToggleVisuals(false);

		Canvas.GoalPanel.gameObject.SetActive(true);
		Canvas.GoldPanel.gameObject.SetActive(true);
	}

	public void ToggleVisuals(bool on)
	{
		var level = World.CurrentLevel;
		if (!level)
			return;

		ToggleVisuals(level.gameObject, on);
	}
}