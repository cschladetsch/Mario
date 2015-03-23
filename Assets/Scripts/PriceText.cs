using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PriceText : MonoBehaviour
{
	public Text Price;
	public Text Shadow;

	public int Amount
	{
		get { return _amount; }
		set { SetAmount(value); }
	}

	private int _amount;

	void Start()
	{
		Price = GetComponent<Text>();
		Shadow = transform.FindChild("Shadow").GetComponent<Text>();
	}

	public void SetAmount(int value)
	{
		_amount = value;
		Price.text = Shadow.text = value.ToString();
	}
}
