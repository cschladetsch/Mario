using Flow;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Something that makes a product given correct ingredients
/// </summary>
public class Cooker : MarioObject
{
	/// <summary>
	/// How to make the product that this cooker... cooks
	/// </summary>
	public Recipe Recipe;

	public ProgressBar ProgressBar;

	public new IKernel Kernel;

	public Color DeselectedColor;

	public Color UnusableColor;

	/// <summary>
	/// True if this is currently selected cooker
	/// </summary>
	public bool Active;

	public IGenerator Generator;

	public IFuture<bool> Future;

	public delegate void CompletedHandler(IngredientType type);

	public event CompletedHandler Completed;

	//public UnityEngine.UI.Text TimerText;

	//private Dictionary<IngredientType, int> _ingredients;

	//private Dictionary<IngredientType, UnityEngine.UI.Text> _counts;

	//public GameObject ProgressBar;

	public UnityEngine.UI.Image ProductImage;

	protected override void Begin()
	{
		base.Begin();

		ProgressBar.TotalTime = Recipe.CookingTime;
		ProgressBar.Reset();

		World.GoalChanged += GoalChanged;
	}

	private void GoalChanged(int index, StageGoal newgoal)
	{
		Debug.Log("Cooker.GoalChanged: " + index + " " + newgoal.Ingredients.Length);
		//var act = index >= 
	}

	protected override void Tick()
	{
		base.Tick();

		//Debug.Log(name + " " + CanCook());
		var c = CanCook() ? 1.0f : 0.5f;
		ProductImage.color = new Color(c, c, c, c);
	}

	public void Select(bool select)
	{
		Debug.Log("Selected " + name + " " + select);
		//_tint.color = DeselectedColor;
	}

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();

		//_ingredients = IngredientItem.CreateIngredientDict<int>();

		GatherIngredientButtons();

		//Kernel = FindObjectOfType<Kernel>().Kern;
	}

	private void GatherIngredientButtons()
	{
		////Debug.Log("GatherIngredientButtons: " + name);
		//_counts = IngredientItem.CreateIngredientDict<UnityEngine.UI.Text>();
		//foreach (Transform tr in transform)
		//{
		//	var ing = tr.GetComponent<IngredientItem>();
		//	if (ing == null)
		//	{
		//		//Debug.Log("No IngredientItem for " + tr.name);
		//		continue;
		//	}

		//	var text = tr.FindChild("Count").GetComponent<UnityEngine.UI.Text>();
		//	_counts[ing.Type] = text;

		//	//Debug.Log("Found Ingredient button for " + ing.Type + ": " + text.text);
		//}
	}

	/// <summary>
	/// Add an ingredient to the recipe
	/// </summary>
	/// <param name="type">the ingredient to add</param>
	/// <returns>true if was added</returns>
	public bool Add(IngredientType type)
	{
		//Debug.Log("Adding " + type + " to " + name);

		//for (var n = 0; n < Recipe.Inventory.Count; ++n)
		//{
		//	if (type != Recipe.Inventory[n]) 
		//		continue;

		//	_ingredients[type]++;
		//	_counts[type].text = _ingredients[type].ToString();

		//	return true;
		//}

		return false;
	}

	/// <summary>
	/// Remove an ingredient from the recipe
	/// </summary>
	/// <param name="type"></param>
	/// <returns>true if ingredient was removed</returns>
	public bool Remove(IngredientType type)
	{
		//if (_ingredients[type] == 0)
		//	return false;
		//_ingredients[type]--;
		return true;
	}

	public IFuture<bool> Cook()
	{
		if (_cooking)
			return null;

		//if (!Recipe.Satisfied(_ingredients))
		//	return null;

		// TODO: WHY OH WHY IS this.Kernel null?
		var k = FindObjectOfType<World>().Kernel;
		var future = k.Factory.NewFuture<bool>();
		Generator = k.Factory.NewCoroutine(Cook, future);
		return future;
	}

	public bool CanCook()
	{
		var satisfied = Recipe.Satisfied(Player.Inventory);
		//Debug.Log("satisfied: " + satisfied + ", cooking: " + _cooking);
		return satisfied && !_cooking;
	}

	private bool _cooking;

	private IEnumerator Cook(IGenerator self, IFuture<bool> done)
	{
		if (_cooking)
		{
			self.Complete();
			yield break;
		}

		ProgressBar.Reset();
		ProgressBar.TotalTime = Recipe.CookingTime;

		_cooking = true;

		var remaining = Recipe.CookingTime;
		while (remaining > 0)
		{
			remaining -= (float) RealDeltaTime;
			ProgressBar.SetPercent(remaining/Recipe.CookingTime);
			yield return 0;
		}

		done.Value = true;

		self.Complete();

		Generator = null;

		if (Completed != null)
			Completed(Recipe.Result);

		var count = Recipe.NumResults;
		//Debug.Log("Cooked " + count + " " + Recipe.Result);
		ProgressBar.Reset();

		Player.CookedItem(Recipe.Result, count);

		UpdateDisplay();

		_cooking = false;
	}

	private void UpdateProgressBar(float t)
	{
		//t = Mathf.Clamp01(t);
		//TimerText.text = string.Format("{0:0.0}", t);

		//var y = ProgressBar.transform.position.y;
		//var len = t*1.25f;

		//ProgressBar.transform.localScale = new Vector3(len, 1, 1);
		//ProgressBar.transform.SetX(len/2.0f);
		////Debug.Log("Cooking " + Recipe.Result + " in " + t);
	}

	//public void RemoveIngredient(GameObject go)
	//{
	//	var type = go.GetComponent<IngredientItem>().Type;
	//	Debug.Log("Remove " + type);
	//	if (!Remove(type))
	//		return;

	//	Player.Inventory[type]++;
	//	UpdateDisplay();
	//}

	private void UpdateDisplay()
	{
		//foreach (IngredientType type in Enum.GetValues(typeof (IngredientType)))
		//{
		//	if (type == IngredientType.None)
		//		continue;

		//	if (_counts[type] == null)
		//		continue;

		//	_counts[type].text = _ingredients[type].ToString();
		//}

		var ui = World.CookingAreaUi;
		ui.InventoryPanel.UpdateDisplay(Player.Inventory, false);
	}

	public void IngredientButtonPressed(IngredientType item)
	{
		Debug.Log("Cooker added " + item);
		// TODO
		//if (Recipe.CanAdd(item))
		//	Recipe.Add(item);
	}

	public void CanUse(bool use)
	{
		Debug.Log("Can use cooker " + name + ": " + use);
	}

	public void StartBake()
	{
		Debug.Log("Start Bake");
	}

	public bool Cooking()
	{
		return _cooking;
	}
}