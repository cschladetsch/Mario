//

namespace Flow
{
	/// <summary>
	/// A Node is a Group that steps all referenced Generators when it itself is Stepped, and similarly for Post.
	/// </summary>
	public interface INode : IGroup
	{
	}
}