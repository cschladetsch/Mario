public class MinMaxTimer : MarioObject
{
	public float MinTime = 1;

	public float MaxTime = 2;

	public bool AutoStart;

	private float _timer;

	protected override void Begin()
	{
		_timer = UnityEngine.Random.Range(MinTime, MaxTime);
	}

	protected override void Tick()
	{
		_timer -= (float) RealDeltaTime;
		if (_timer > 0)
			return;
	}
}