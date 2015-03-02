using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaBase : MarioObject
{
	public GameObject UiCanvas;

	public GameObject NextButton;

	protected override void Construct()
	{
		base.Construct();
	}

	public virtual void Next()
	{
		Debug.Log("Base Next");
	}

	protected override void Begin()
	{
		base.Begin();
		if (UiCanvas)
			UiCanvas.SetActive(false);
	}
	public void NextPressed()
	{
		Debug.Log("AreaBase.NextPressed");
		Next();
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
}


