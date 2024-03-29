//

namespace Flow
{
	/// <summary>
	/// Trigger handler.
	/// </summary>
	public delegate void TriggerHandler(ITrigger trigger, ITransient reason);

	/// <summary>
	/// A Trigger Completes itself when any of the objects in it are Completed
	/// </summary>
	public interface ITrigger : IGroup
	{
		/// <summary>
		/// Occurs when any of the objects added to the trigger are deleted.
		/// </summary>
		event TriggerHandler Tripped;

		/// <summary>
		/// Gets the Transient that triggered this instance.
		/// </summary>
		/// <value>
		/// The reason why this trigger was tripped (and hence Completed).
		/// </value>
		ITransient Reason { get; }
	}
}