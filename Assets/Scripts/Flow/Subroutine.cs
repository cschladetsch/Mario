//

namespace Flow
{
	internal class Subroutine<TR> : TypedGenerator<TR>, ISubroutine<TR>
	{
		/// <inheritdoc />
		public override void Step()
		{
			if (!Active || !Running)
				return;

			if (Sub == null)
			{
				Complete();
				return;
			}

			Value = Sub(this);

			base.Step();
		}

		internal Func<IGenerator, TR> Sub;
	}
}