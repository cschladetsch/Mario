//

namespace Flow
{
	internal class Future<T> : Transient, IFuture<T>
	{
		/// <inheritdoc />
		public event FutureHandler<T> Arrived;

		/// <inheritdoc />
		public bool Available { get; private set; }

		/// <inheritdoc />
		public T Value
		{
			get
			{
				if (!Available)
					throw new FutureNotSetException();

				return _value;
			}
			set
			{
				if (Available)
					throw new FutureAlreadySetException();

				_value = value;
				Available = true;

				if (Arrived != null)
					Arrived(this);

				Complete();
			}
		}

		private T _value;
	}
}