using ProgressBar;
using UnityEngine;
using System.Collections;

public class TestProgressBar : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public float Speed = 10;

	private float _val;
	
	// Update is called once per frame
	void Update ()
	{
		//var pg = GetComponent<ProgressRadialBehaviour>();
		var pg = GetComponent<ProgressBarBehaviour>();
		_val += Time.deltaTime;
		pg.Value = _val;

		Debug.Log(pg.Value);
	}
}
