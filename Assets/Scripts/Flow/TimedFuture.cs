//

using System;

namespace Flow
{
	internal class TimedFuture<T> : Future<T>, ITimedFuture<T>
	{
		/// <inheritdoc />
		public event TimedOutHandler TimedOut;

		/// <inheritdoc />
		public ITimer Timer { get; internal set; }

		/// <inheritdoc />
		public bool HasTimedOut { get; protected set; }

		internal TimedFuture(ITransient kernel, TimeSpan span)
		{
			Timer = kernel.Factory.NewTimer(span);
			Timer.Elapsed += HandleElapsed;
		}

		private void HandleElapsed(ITransient sender)
		{
			if (!Active)
				return;

			if (TimedOut != null)
				TimedOut(this);

			HasTimedOut = true;

			Complete();
		}
	}
}