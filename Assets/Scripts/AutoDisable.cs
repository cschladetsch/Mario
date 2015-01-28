using UnityEngine;

public class AutoDisable : MonoBehaviour
{
	void Awake()
	{
		gameObject.SetActive(false);
	}
}
