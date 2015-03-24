using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PriceText : MarioObject
{
	public Text Price;
	public Text Shadow;

	public int Amount
	{
		get { return _amount; }
		set { SetAmount(value); }
	}

	private int _amount;

	protected override void BeforeFirstUpdate()
	{
		base.BeforeFirstUpdate();
		Price = GetComponent<Text>();
		Shadow = transform.FindChild("Shadow").GetComponent<Text>();
		//Debug.Log(string.Format("Price {0} Shadow {1} name {2}", Price, Shadow, name));
	}

	public void SetAmount(int value)
	{
		_amount = value;
		// TODO WTD
		if (Price == null)
			BeforeFirstUpdate();

		Price.text = Shadow.text = value.ToString();
	}
}