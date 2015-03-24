//

using System;

namespace Flow
{
	/// <summary>
	/// A one-shot Timer that will fire its Elapsed event, and then CompleteDeliveryToFactory itself after a fixed time Interval.
	/// </summary>
	public interface ITimer : IPeriodic
	{
		/// <summary>
		/// Gets the game time that the timer will elapse and subsequently delete itself.
		/// </summary>
		/// <value>
		/// The soonest time that the timer will elapse
		/// </value>
		DateTime TimeEnds { get; }
	}
}