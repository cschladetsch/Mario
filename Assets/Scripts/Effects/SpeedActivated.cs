public class SpeedActivated : ScalarDependantEffect
{
	public override bool Triggered()
	{
		return false;
		//return _player.Speed > Value;
	}
}