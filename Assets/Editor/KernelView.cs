using UnityEngine;
using UnityEditor;

public class KernelView : EditorWindow
{
	string myString = "Hello World";
	bool groupEnabled;
	bool myBool = true;
	float myFloat = 1.23f;

	private static KernelView _window;

	// Add menu named "My Window" to the Window menu
	[MenuItem("PlaySide/Kernel View")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		_window = (KernelView)EditorWindow.GetWindow(typeof(KernelView));
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		myString = EditorGUILayout.TextField("Text Field", myString);

		groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
		myBool = EditorGUILayout.Toggle("Toggle", myBool);
		myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup();
	}
}