//

using System;
using System.Collections.Generic;

namespace Flow
{
	/// <inheritdoc />
	internal class TypedCoroutine<TR> : TypedGenerator<TR>, ITypedCoroutine<TR>
	{
		/// <inheritdoc />
		public override void Step()
		{
			if (!Running || !Active)
				return;

			if (_state == null)
			{
				if (Start == null)
					CannotStart();

				_state = Start();
				if (_state == null)
					CannotStart();
			}

			if (!_state.MoveNext())
			{
				Complete();
				return;
			}

			Value = _state.Current;
			base.Step();
		}

		private void CannotStart()
		{
			throw new Exception("TypedCoroutine cannot start");
		}

		private IEnumerator<TR> _state;

		internal Func<IEnumerator<TR>> Start;
	}
}