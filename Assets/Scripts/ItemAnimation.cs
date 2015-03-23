using System.Collections;
using Flow;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemAnimation : MarioObject
{
	protected override void Construct()
	{
	}

	protected override void Begin()
	{
	}

	protected override void Tick()
	{
	}

	public delegate void CallbackHandler(IngredientType type);

	struct Args
	{
		public IngredientType Type;
		public GameObject From;
		public GameObject To;
		public float Time;
		public World World;
		public CallbackHandler Callback;
	}

	public static IGenerator Animate(IngredientType type, GameObject from, GameObject to, float time, CallbackHandler cb = null)
	{
		var world = FindObjectOfType<World>();
		return world
			.Kernel.Factory.NewCoroutine(AnimateItemCoro, new Args {World = world, Type = type, From = from, To = to, Time = time, Callback = cb});
	}

	static IEnumerator AnimateItemCoro(IGenerator self, Args args)
	{
		var item = (GameObject)Instantiate(args.World.GetInfo(args.Type).ImagePrefab);
		item.transform.SetParent(World.Canvas.transform);

		var start = args.From.transform.position;
		var end = args.To.transform.position;
		var mid = start + (end - start)/2.0f;
		var len = (start - end).magnitude;
		var r = len/10.0f;
		mid.x += Random.Range(-r, r);
		mid.y += Random.Range(-r, r);

		var para = new ParabolaUI(start, mid, end, args.Time);

		while (true)
		{
			if (para.Completed)
			{
				if (args.Callback != null)
					args.Callback(args.Type);

				Destroy(item);
				yield break;
			}

			item.transform.position = para.UpdatePos();

			yield return 0;
		}
	}
}
