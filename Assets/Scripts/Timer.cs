using UnityEngine;

/// <summary>
/// A generic timer system
/// </summary>
public class Timer : MonoBehaviour
{
	/// <summary>
	/// The target object to trigger
	/// </summary>
	public GameObject Target;

	/// <summary>
	/// The length of the timer
	/// </summary>
	public float Duration;

	/// <summary>
	/// If false, the timer will fire once then destroy itself
	/// </summary>
	public bool Repeating = true;

	/// <summary>
	/// The time till next trigger
	/// </summary>
	private float _time;

	private void Start()
	{
		_time = Duration;
	}

	private void Update()
	{
		_time -= Time.deltaTime;
		if (_time > 0)
			return;

		_time = Duration;

		if (Target)
			Target.BroadcastMessage("Trigger");
	}
}