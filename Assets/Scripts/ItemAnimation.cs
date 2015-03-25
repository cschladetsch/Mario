using System.Collections;
using Flow;
using UnityEngine;
using UnityEngine.UI;

public class ItemAnimation : MarioObject
{
	public delegate void CallbackHandler(IngredientType type);

	private struct Args
	{
		public IngredientType Type;
		public GameObject From;
		public GameObject To;
		public float Time;
		public World World;
		public CallbackHandler Callback;
	}

	public static IGenerator Animate(IngredientType type, GameObject from, GameObject to, float time,
		CallbackHandler cb = null)
	{
		var world = FindObjectOfType<World>();
		return world
			.Kernel.Factory.NewCoroutine(AnimateItemCoro,
				new Args {World = world, Type = type, From = from, To = to, Time = time, Callback = cb});
	}

	private static IEnumerator AnimateItemCoro(IGenerator self, Args args)
	{
		if (!args.From)
		{
			Debug.Log("AnimateItemCoro: From is null");
			yield break;
		}
		if (!args.To)
		{
			Debug.Log("AnimateItemCoro: To is null");
			yield break;
		}

		var item = (GameObject) Instantiate(args.World.GetInfo(args.Type).ImagePrefab);
		if (item == null)
		{
			Debug.LogWarning("Couldn't make image for " + args.Type);
			yield break;
		}

		var rc = item.GetRectTransform();
		var scale = 0.15f;
		var size = Mathf.Min(Screen.width*scale, Screen.height*scale);
		rc.sizeDelta = new Vector2(size, size);
		
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