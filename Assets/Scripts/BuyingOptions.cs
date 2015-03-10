using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for buying options panel
/// </summary>
public class BuyingOptions : MarioObject
{
	public UnityEngine.UI.Button Cherry;
	public UnityEngine.UI.Button Muffin;
	public UnityEngine.UI.Button Chock;
	public UnityEngine.UI.Button Mint;

	void Update ()
	{
		var lev = World.GoalIndex;

		// TODO: automate based on requirements for current goal results. this will require
		// a bit of a refactor, so this hack stands for the moment.
		Cherry.interactable = lev >= 0;
		Muffin.interactable = lev >= 0;
		Chock.interactable = lev >= 1;
		Mint.interactable = lev >= 1;
	}
}
