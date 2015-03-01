using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Kernel))]
public class KernelView : Editor
{
	readonly Dictionary<Guid, bool> _show = new Dictionary<Guid, bool>(); 

	public override void OnInspectorGUI()
	{
		var k = (Kernel)target;
		if (k == null)
			return;

		var kernel = k.Kern;
		var root = kernel.Root;

		EditorGUILayout.LabelField("Root", root.Name);

		if (!_show.ContainsKey(root.Guid))
			_show.Add(root.Guid, true);

		_show[root.Guid] = EditorGUILayout.Foldout(_show[root.Guid], root.Name);
		if (_show[root.Guid])
		{
			foreach (var ch in root.Contents)
			{
				_show[ch.Guid] = EditorGUILayout.Foldout(_show[ch.Guid], ch.Name);
			}
		}


	}
}


