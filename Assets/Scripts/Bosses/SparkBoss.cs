using System;
using System.Collections;
using Flow;
using UnityEngine;

public class SparkBoss : Boss
{
	public GameObject SparkPrefab;

	private Spark _spark;

	protected override void Construct()
	{
	}

	protected override void Begin()
	{
		base.Begin();

		Kernel.Factory.NewCoroutine(SparkMove);
	}

	IEnumerator SparkMove(IGenerator self)
	{
		Debug.Log("SparkMove " + World.CurrentLevel.Conveyors.Count);
		_spark = ((GameObject) Instantiate(SparkPrefab)).GetComponent<Spark>();

		for (var n = World.CurrentLevel.Conveyors.Count - 1; n > 0; --n)
		{
			Debug.Log("Spark Move " + n);
			_spark.Height = n;
			_spark.transform.SetY(World.CurrentLevel.Conveyors[n].transform.position.y);

			yield return self.ResumeAfter(TimeSpan.FromSeconds(1));
		}

		Destroy(gameObject);
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
