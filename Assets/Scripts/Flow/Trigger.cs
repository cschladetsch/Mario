//

namespace Flow
{
	/// <summary>
	/// A trigger is a group that deletes itself when any of its children are deleted
	/// </summary>
	internal class Trigger : Group, ITrigger
	{
		/// <inheritdoc />
		public event TriggerHandler Tripped;

		/// <inheritdoc />
		public ITransient Reason { get; private set; }

		internal Trigger()
		{
			Removed += Trip;
		}

		private void Trip(IGroup self, ITransient other)
		{
			Reason = other;

			if (Tripped != null)
				Tripped(this, other);

			Complete();
		}
	}
}