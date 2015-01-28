using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An EffectGroup contains a set of ScalarDependantEffects as sub-objects.
/// At most one of these Effects will be active at any given time,
/// determined as the one which could be active with the highest Value.
/// </summary>
public class EffectGroup : MonoBehaviour
{
	private List<ScalarDependantEffect> _effects = new List<ScalarDependantEffect>();

	void Start()
	{
		_effects = transform.GetComponentsInChildren<ScalarDependantEffect>().ToList();
		_effects.Sort((a, b) => a.Value.CompareTo(b.Value));
	}

	void Update()
	{
		if (_effects.Count == 0)
			return;

		// find the effect which should be shown and
		// has the highest Value.
		Effect effect = null;
		float max = float.MinValue;
		foreach (var e in _effects)
		{
			if (e.Value < max || !e.ShouldShow())
				continue;

			max = e.Value;
			effect = e;
		}

		foreach (var e in _effects)
			e.Activate(e == effect);
	}
}
