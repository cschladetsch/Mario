using System;

#pragma warning disable 414

namespace Flow
{
	public class Transient : ITransient
	{
		public Guid Guid { get; private set; }

		/// <inheritdoc />
		public event TransientHandler Completed;

		/// <summary>
		///  Occurs when completed, with a reason why. 
		/// </summary>
		public event TransientHandlerReason WhyCompleted;

		/// <inheritdoc />
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name == value)
					return;

				if (NewName != null)
					NewName(this, _name, value);

				_name = value;
			}
		}

		/// <inheritdoc />
		public IKernel Kernel { get; /*internal*/ set; }

		/// <inheritdoc />
		public IFactory Factory
		{
			get { return Kernel.Factory; }
		}

		/// <inheritdoc />
		public event NamedHandler NewName;

		/// <inheritdoc />
		public bool Active { get; private set; }

		/// <summary>
		/// Return true if the given other transient is either null or does not exist
		/// </summary>
		/// <returns>
		/// True if the given other transient is either null or does not exist
		/// </returns>
		/// <param name='other'>
		/// The transient to consider
		/// </param>
		public static bool IsNullOrEmpty(ITransient other)
		{
			return other == null || !other.Active;
		}

		private System.Guid _guid;

		public Transient()
		{
			Active = true;

			_guid = System.Guid.NewGuid();
		}

		public static bool DebugTrace;

		/// <inheritdoc />
		public void Complete()
		{
			if (!Active)
				return;

			if (Completed != null)
				Completed(this);

			Active = false;
		}

		/// <inheritdoc />
		public void CompleteAfter(ITransient other)
		{
			if (!Active)
				return;

			if (other == null)
				return;

			if (!other.Active)
			{
				Complete();
				return;
			}

			other.Completed += tr => CompletedBecause(other);
		}

		private void CompletedBecause(ITransient other)
		{
			if (!Active)
				return;

			if (WhyCompleted != null)
				WhyCompleted(this, other);

			Complete();
		}

		/// <inheritdoc />
		public void CompleteAfter(TimeSpan span)
		{
			CompleteAfter(Factory.NewTimer(span));
		}

		private string _name;
	}
}