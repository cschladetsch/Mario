using UnityEngine;
using System.Collections;

public class FactoryAreaUI : MarioObject
{
	public void TruckPressed()
	{
		var truck = FindObjectOfType<Truck>();
		Debug.Log("Truck Pressed");
		truck.Deliver();
	}
}
