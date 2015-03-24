using UnityEngine;

/// <summary>
/// The UI for the selling/purchasing area
/// </summary>
public class CookingAreaUI : MarioObject
{
	/// <summary>
	/// The visuals for the current inventory in the canvas
	/// </summary>
	public InventoryPanel InventoryPanel;

	public ProductsPanelScript ProductsPanel;

	public UnityEngine.UI.Toggle SkipToggle;

	public UnityEngine.UI.Button ShopButton;

	public Cooker MintIceCreamCooker;

	public Cooker CupCakeCooker;

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		ProductsPanel = FindObjectOfType<ProductsPanelScript>();

		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	private void RemoveItemFromPlayer(IngredientType item)
	{
		Player.Inventory[item]--;
		InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	public void BackToShop()
	{
		World.ChangeArea(0);
	}

	private void OnDisable()
	{
		//Debug.Log("BakeryArea enabled: " + gameObject.activeSelf);
	}

	public void RemoveIngredient(GameObject go)
	{
	}
}