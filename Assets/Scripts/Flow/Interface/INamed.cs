//

namespace Flow
{
	/// <summary>
	/// Named handler.
	/// </summary>
	public delegate void NamedHandler(INamed named, string newName, string oldName);

	/// <summary>
	/// Fires its NewName event when its Name property is changed.
	/// </summary>
	public interface INamed
	{
		/// <summary>
		/// Occurs when this instance is given a new name.
		/// </summary>
		event NamedHandler NewName;

		/// <summary>
		/// Gets or sets the name of this instance.
		/// </summary>
		/// <value>
		/// The name of this instance.
		/// </value>
		string Name { get; set; }
	}
}