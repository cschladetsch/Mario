using UnityEngine.UI;

/// <summary>
/// Controller for the persistent set of buttons at the top-right of the screen
/// to move between game areas
/// </summary>
public class ButtonsPanel : MarioObject
{
	public Button ShopButton;
	public Button FactoryButton;
	public Button BakeryButton;

	public void MoveToBakery()
	{
		World.MoveTo(AreaType.Bakery);
	}

	public void MoveToFactory()
	{
		World.MoveTo(AreaType.Factory);
	}

	//public void MoveToShop()
	//{
	//	World.MoveTo(AreaType.Shop);
	//}
}