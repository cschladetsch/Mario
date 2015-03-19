public class HeightActivated : ScalarDependantEffect
{
	public override bool Triggered()
	{
		return false;
		//return Value >= _player.Height;
	}
}