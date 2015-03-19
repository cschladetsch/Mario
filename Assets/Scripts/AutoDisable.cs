using UnityEngine;

public class AutoDisable : MonoBehaviour
{
	private void Awake()
	{
		gameObject.SetActive(false);
	}
}