//

using System;

namespace Flow
{
	/// <inheritdoc />
	internal class Periodic : Subroutine<bool>, IPeriodic
	{
		/// <inheritdoc />
		public event TransientHandler Elapsed;

		/// <inheritdoc />
		public DateTime TimeStarted { get; set; }

		/// <inheritdoc />
		public TimeSpan Interval { get; set; }

		internal Periodic(IKernel kernel, TimeSpan interval)
		{
			Interval = interval;
			TimeStarted = kernel.Time.Now;
			_expires = TimeStarted + Interval;
			Sub = StepTimer;
		}

		private bool StepTimer(IGenerator self)
		{
			if (Kernel.Time.Now < _expires)
				return true;

			if (Elapsed != null)
				Elapsed(this);

			_expires = Kernel.Time.Now + Interval;

			return true;
		}

		private DateTime _expires;
	}
}