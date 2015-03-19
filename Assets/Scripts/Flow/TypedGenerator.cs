namespace Flow
{
	/// <inheritdoc />
	internal abstract class TypedGenerator<TR> : Generator, ITypedGenerator<TR>
	{
		public TR Value { get; protected set; }
	}
}