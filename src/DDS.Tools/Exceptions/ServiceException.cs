// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Exceptions;

/// <summary>
/// The service exception class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ServiceException"/> class with a specified
/// error message and a reference to the inner exception that is the cause of this exception.
/// </remarks>
/// <inheritdoc/>
internal sealed class ServiceException(string? message, Exception? innerException) : Exception(message, innerException)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceException"/> class with a specified
	/// error message
	/// </summary>
	/// <inheritdoc/>
	public ServiceException(string? message) : this(message, null)
	{ }
}
