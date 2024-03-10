namespace DDS.Tools.Exceptions;

/// <summary>
/// The command exception class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandException"/> class with a specified
/// error message and a reference to the inner exception that is the cause of this exception.
/// </remarks>
/// <inheritdoc/>
internal sealed class CommandException(string? message, Exception? innerException) : Exception(message, innerException)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CommandException"/> class with a specified
	/// error message
	/// </summary>
	/// <inheritdoc/>
	public CommandException(string? message) : this(message, null)
	{ }
}
