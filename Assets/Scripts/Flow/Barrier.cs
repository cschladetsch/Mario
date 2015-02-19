//

using System.Linq;

namespace Flow
{
	/// <inheritdoc />
	internal class Barrier : Group, IBarrier
	{
		/// <inheritdoc />
		public override void Post()
		{
			base.Post();

			if (Contents.Any(t => t.Active))
				return;

			if (Additions.Count == 0)
				Complete();
		}
	}
}